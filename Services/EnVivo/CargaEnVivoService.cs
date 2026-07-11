using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public class CargaEnVivoService(ApplicationDbContext db)
    {
        private readonly MotorRally _motor = new();

        public async Task<PartidoEnVivo> CrearPartidoAsync(
            int dupla1Id, int dupla2Id, string torneo, DateTime fecha)
        {
            var partido = new PartidoEnVivo
            {
                Dupla1Id = dupla1Id,
                Dupla2Id = dupla2Id,
                Torneo = torneo,
                Fecha = fecha.ToUniversalTime()
            };
            db.PartidosEnVivo.Add(partido);
            await db.SaveChangesAsync();
            return partido;
        }

        public async Task<SetEnVivo> IniciarSetAsync(
            int partidoId, int numeroSet,
            int sacadorInicialD1, int sacadorInicialD2,
            int duplaQueSacaPrimeroId)
        {
            var set = new SetEnVivo
            {
                PartidoEnVivoId = partidoId,
                NumeroSet = numeroSet,
                SacadorInicialDupla1JugadorId = sacadorInicialD1,
                SacadorInicialDupla2JugadorId = sacadorInicialD2,
                DuplaQueSacaPrimeroId = duplaQueSacaPrimeroId
            };
            db.SetsEnVivo.Add(set);
            await db.SaveChangesAsync();
            return set;
        }

        public async Task<Rally> AbrirRallyAsync(int setEnVivoId)
        {
            int count = await db.Rallies.CountAsync(r => r.SetEnVivoId == setEnVivoId);

            var ultimoCerrado = await db.Rallies
                .Where(r => r.SetEnVivoId == setEnVivoId && r.DuplaGanadoraId != null)
                .OrderByDescending(r => r.NumeroRally)
                .FirstOrDefaultAsync();

            var rally = new Rally
            {
                SetEnVivoId = setEnVivoId,
                NumeroRally = count + 1,
                MarcadorDupla1 = ultimoCerrado?.MarcadorDupla1 ?? 0,
                MarcadorDupla2 = ultimoCerrado?.MarcadorDupla2 ?? 0
            };
            db.Rallies.Add(rally);
            await db.SaveChangesAsync();
            return rally;
        }

        public async Task<ResultadoRegistro> RegistrarAccionAsync(
            int rallyId, CargaAccion carga, object? detalle)
        {
            if (detalle != null)
            {
                bool valido = carga.Fundamento switch
                {
                    Fundamento.Saque     => detalle is DetalleSaque,
                    Fundamento.Recepcion => detalle is DetalleRecepcion,
                    Fundamento.Ataque    => detalle is DetalleAtaque,
                    _                    => false
                };
                if (!valido)
                    throw new ArgumentException(
                        $"Detalle no corresponde al fundamento {carga.Fundamento}");
            }

            var ctx = await ArmarContextoAsync(rallyId);
            var sugerencia = _motor.SugerirProximoPaso(ctx);

            var accion = new Accion
            {
                RallyId    = rallyId,
                Secuencia  = ctx.AccionesRallyActual.Count + 1,
                JugadorId  = carga.JugadorId,
                Fundamento = carga.Fundamento,
                Calidad    = carga.Calidad,
                Complejo   = sugerencia.Complejo,
                EsDe2da    = carga.EsDe2da,
                EsRejuego  = sugerencia.EsRejuego,
                FechaHora  = DateTime.UtcNow
            };

            if (detalle is DetalleSaque ds)       accion.DetalleSaque    = ds;
            else if (detalle is DetalleRecepcion dr) accion.DetalleRecepcion = dr;
            else if (detalle is DetalleAtaque da)    accion.DetalleAtaque   = da;

            db.Acciones.Add(accion);
            await db.SaveChangesAsync();

            var resultado = _motor.Registrar(ctx, carga);

            if (resultado.Derivacion != null)
            {
                var der = resultado.Derivacion;
                var target = await db.Acciones
                    .Where(a => a.RallyId == rallyId
                             && a.Fundamento == der.FundamentoDerivado
                             && a.Calidad == null)
                    .OrderByDescending(a => a.Secuencia)
                    .FirstOrDefaultAsync();

                if (target != null)
                {
                    target.Calidad = der.CalidadDerivada;
                    await db.SaveChangesAsync();
                }
            }

            if (resultado.Cierre != null)
                await AplicarCierreAsync(rallyId, resultado.Cierre.DuplaGanadoraId, ctx);

            return resultado;
        }

        public async Task<ResultadoRegistro> CierreDirectoAsync(
            int rallyId, TipoCierreDirecto tipo, int? duplaGanadoraId = null)
        {
            var ctx = await ArmarContextoAsync(rallyId);
            var resultado = _motor.CierreDirecto(ctx, tipo, duplaGanadoraId);
            await AplicarCierreAsync(rallyId, resultado.Cierre!.DuplaGanadoraId, ctx);
            return resultado;
        }

        private async Task AplicarCierreAsync(int rallyId, int duplaGanadoraId, ContextoRally ctx)
        {
            var rally = await db.Rallies.FindAsync(rallyId)
                ?? throw new InvalidOperationException($"Rally {rallyId} no encontrado");

            rally.DuplaGanadoraId  = duplaGanadoraId;
            rally.MarcadorDupla1  += duplaGanadoraId == ctx.Dupla1Id ? 1 : 0;
            rally.MarcadorDupla2  += duplaGanadoraId == ctx.Dupla2Id ? 1 : 0;

            await db.SaveChangesAsync();
        }

        private async Task<ContextoRally> ArmarContextoAsync(int rallyId)
        {
            var rally = await db.Rallies
                .Include(r => r.SetEnVivo)
                    .ThenInclude(s => s.PartidoEnVivo)
                .Include(r => r.Acciones)
                .FirstOrDefaultAsync(r => r.Id == rallyId)
                ?? throw new InvalidOperationException($"Rally {rallyId} no encontrado");

            var partido = rally.SetEnVivo.PartidoEnVivo!;

            var dupla1 = await db.Duplas
                .Include(d => d.Jugador1)
                .Include(d => d.Jugador2)
                .FirstAsync(d => d.Id == partido.Dupla1Id);

            var dupla2 = await db.Duplas
                .Include(d => d.Jugador1)
                .Include(d => d.Jugador2)
                .FirstAsync(d => d.Id == partido.Dupla2Id);

            var ganadores = await db.Rallies
                .Where(r => r.SetEnVivoId == rally.SetEnVivoId
                         && r.DuplaGanadoraId != null
                         && r.NumeroRally < rally.NumeroRally)
                .OrderBy(r => r.NumeroRally)
                .Select(r => r.DuplaGanadoraId!.Value)
                .ToListAsync();

            var combinadas = await db.ModificadoresCombinadas.ToListAsync();

            IReadOnlyDictionary<int, IReadOnlyList<JugadorEnCancha>> jugadoresPorDupla =
                new Dictionary<int, IReadOnlyList<JugadorEnCancha>>
                {
                    [dupla1.Id] = new[]
                    {
                        new JugadorEnCancha(dupla1.Jugador1!.Id, dupla1.Jugador1.Posicion),
                        new JugadorEnCancha(dupla1.Jugador2!.Id, dupla1.Jugador2.Posicion)
                    },
                    [dupla2.Id] = new[]
                    {
                        new JugadorEnCancha(dupla2.Jugador1!.Id, dupla2.Jugador1.Posicion),
                        new JugadorEnCancha(dupla2.Jugador2!.Id, dupla2.Jugador2.Posicion)
                    }
                };

            return new ContextoRally
            {
                Dupla1Id   = dupla1.Id,
                Dupla2Id   = dupla2.Id,
                JugadoresPorDupla              = jugadoresPorDupla,
                SacadorInicialDupla1JugadorId  = rally.SetEnVivo.SacadorInicialDupla1JugadorId,
                SacadorInicialDupla2JugadorId  = rally.SetEnVivo.SacadorInicialDupla2JugadorId,
                DuplaQueSacaPrimeroId          = rally.SetEnVivo.DuplaQueSacaPrimeroId,
                GanadoresRalliesPrevios        = ganadores,
                AccionesRallyActual            = rally.Acciones.OrderBy(a => a.Secuencia).ToList(),
                Combinadas                     = combinadas
            };
        }
    }
}
