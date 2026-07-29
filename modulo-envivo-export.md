# Modulo EnVivo - export de codigo

## Models\EnVivo\Accion.cs
```csharp
using System.ComponentModel.DataAnnotations;
using SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class Accion
    {
        public int Id { get; set; }

        [Required]
        public int RallyId { get; set; }
        public Rally? Rally { get; set; }

        [Required]
        [Display(Name = "Secuencia")]
        public int Secuencia { get; set; }

        [Required]
        [Display(Name = "Jugador")]
        public int JugadorId { get; set; }
        public Jugador? Jugador { get; set; }

        [Required]
        public Fundamento Fundamento { get; set; }

        public Calidad? Calidad { get; set; }

        [Required]
        public Complejo Complejo { get; set; }

        public bool EsDe2da { get; set; }

        public bool EsRejuego { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        public DetalleSaque? DetalleSaque { get; set; }
        public DetalleRecepcion? DetalleRecepcion { get; set; }
        public DetalleAtaque? DetalleAtaque { get; set; }
    }
}
```

## Models\EnVivo\Calidad.cs
```csharp
namespace SandStats.Models.EnVivo
{
    public enum Calidad
    {
        DobleNegativo = 0,
        Negativo = 1,
        Slash = 2,
        Exclamativa = 3,
        Positivo = 4,
        DoblePositivo = 5
    }
}
```

## Models\EnVivo\Complejo.cs
```csharp
namespace SandStats.Models.EnVivo
{
    public enum Complejo
    {
        K1,
        K2
    }
}
```

## Models\EnVivo\DetalleAtaque.cs
```csharp
using System.ComponentModel.DataAnnotations;
using SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class DetalleAtaque
    {
        public int Id { get; set; }

        [Required]
        public int AccionId { get; set; }
        public Accion? Accion { get; set; }

        [Required]
        [Display(Name = "Lado")]
        public TipoLado Lado { get; set; }

        [Required]
        [Display(Name = "Tipo de AcciÃ³n")]
        public TipoAcciones TipoAccion { get; set; }

        [Required]
        [Display(Name = "Zona Destino")]
        public ZonaCancha ZonaDestino { get; set; }

        [Display(Name = "Es Varilla")]
        public bool EsVarilla { get; set; }

        [Display(Name = "Es Especial")]
        public bool EsEspecial { get; set; }
    }
}
```

## Models\EnVivo\DetalleRecepcion.cs
```csharp
using System.ComponentModel.DataAnnotations;
using SandStats.Models.SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class DetalleRecepcion
    {
        public int Id { get; set; }

        [Required]
        public int AccionId { get; set; }
        public Accion? Accion { get; set; }

        [Required]
        [Display(Name = "Tipo de RecepciÃ³n")]
        public TipoRecepcion TipoRecepcion { get; set; }
    }
}
```

## Models\EnVivo\DetalleSaque.cs
```csharp
using System.ComponentModel.DataAnnotations;
using SandStats.Models.SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class DetalleSaque
    {
        public int Id { get; set; }

        [Required]
        public int AccionId { get; set; }
        public Accion? Accion { get; set; }

        [Required]
        [Display(Name = "Zona de Saque")]
        public ZonaSaque ZonaSaque { get; set; }

        [Required]
        [Display(Name = "Tipo de Saque")]
        public TipoSaque TipoSaque { get; set; }
    }
}
```

## Models\EnVivo\Fundamento.cs
```csharp
namespace SandStats.Models.EnVivo
{
    public enum Fundamento
    {
        Saque,
        Recepcion,
        Ataque,
        Bloqueo,
        Defensa,
        Armado,
        FreeBall
    }
}
```

## Models\EnVivo\ModificadorCombinada.cs
```csharp
namespace SandStats.Models.EnVivo
{
    public class ModificadorCombinada
    {
        public int Id { get; set; }

        public Fundamento FundamentoCargado { get; set; }

        public Calidad CalidadCargada { get; set; }

        public Fundamento FundamentoDerivado { get; set; }

        public Calidad? CalidadDerivada { get; set; }
    }
}
```

## Models\EnVivo\PartidoEnVivo.cs
```csharp
using System.ComponentModel.DataAnnotations;
using SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class PartidoEnVivo
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Torneo")]
        public string Torneo { get; set; } = string.Empty;

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [Display(Name = "Dupla 1")]
        public int Dupla1Id { get; set; }
        public Dupla? Dupla1 { get; set; }

        [Required]
        [Display(Name = "Dupla 2")]
        public int Dupla2Id { get; set; }
        public Dupla? Dupla2 { get; set; }

        public List<SetEnVivo> Sets { get; set; } = new();
    }
}
```

## Models\EnVivo\Rally.cs
```csharp
using System.ComponentModel.DataAnnotations;
using SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class Rally
    {
        public int Id { get; set; }

        [Required]
        public int SetEnVivoId { get; set; }
        public SetEnVivo? SetEnVivo { get; set; }

        [Required]
        [Display(Name = "NÃºmero de Rally")]
        public int NumeroRally { get; set; }

        [Display(Name = "Dupla Ganadora")]
        public int? DuplaGanadoraId { get; set; }
        public Dupla? DuplaGanadora { get; set; }

        public int MarcadorDupla1 { get; set; }
        public int MarcadorDupla2 { get; set; }

        public TipoCierreRally? TipoCierre { get; set; }

        public List<Accion> Acciones { get; set; } = new();
    }
}
```

## Models\EnVivo\SetEnVivo.cs
```csharp
using System.ComponentModel.DataAnnotations;
using SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class SetEnVivo
    {
        public int Id { get; set; }

        [Required]
        public int PartidoEnVivoId { get; set; }
        public PartidoEnVivo? PartidoEnVivo { get; set; }

        [Required]
        [Display(Name = "NÃºmero de Set")]
        public int NumeroSet { get; set; }

        [Required]
        [Display(Name = "Sacador inicial Dupla 1")]
        public int SacadorInicialDupla1JugadorId { get; set; }
        public Jugador? SacadorInicialDupla1Jugador { get; set; }

        [Required]
        [Display(Name = "Sacador inicial Dupla 2")]
        public int SacadorInicialDupla2JugadorId { get; set; }
        public Jugador? SacadorInicialDupla2Jugador { get; set; }

        [Required]
        [Display(Name = "Dupla que saca primero")]
        public int DuplaQueSacaPrimeroId { get; set; }
        public Dupla? DuplaQueSacaPrimero { get; set; }

        public int? DuplaGanadoraId { get; set; }
        public Dupla? DuplaGanadora { get; set; }

        public List<Rally> Rallies { get; set; } = new();
    }
}
```

## Models\EnVivo\TipoCierreRally.cs
```csharp
namespace SandStats.Models.EnVivo
{
    public enum TipoCierreRally { PorJuego, Ace, ErrorSaque, ErrorVario, CierreRapido }
}
```

## Models\EnVivo\ZonaCancha.cs
```csharp
namespace SandStats.Models.EnVivo
{
    public enum ZonaCancha
    {
        Zona1 = 1,
        Zona2 = 2,
        Zona3 = 3,
        Zona4 = 4,
        Zona5 = 5,
        Zona6 = 6,
        Zona7 = 7,
        Zona8 = 8,
        Zona9 = 9
    }
}
```

## Services\EnVivo\CargaAccion.cs
```csharp
using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public record CargaAccion(Fundamento Fundamento, Calidad? Calidad, int JugadorId, bool EsDe2da);
}
```

## Services\EnVivo\CargaEnVivoService.cs
```csharp
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

            var rallyEntity = (await db.Rallies.FindAsync(rallyId))!;
            if (rallyEntity.DuplaGanadoraId != null)
                throw new InvalidOperationException("El rally ya estÃ¡ cerrado");

            var sugerencia = _motor.SugerirProximoPaso(ctx);

            if (carga.Fundamento is Fundamento.Recepcion or Fundamento.Bloqueo or Fundamento.Defensa)
            {
                if (!carga.Calidad.HasValue ||
                    !ctx.Combinadas.Any(c => c.FundamentoCargado == carga.Fundamento
                                          && c.CalidadCargada    == carga.Calidad.Value))
                    throw new ArgumentException(
                        $"{carga.Fundamento} con calidad {carga.Calidad?.ToString() ?? "null"} " +
                        $"no es una combinaciÃ³n vÃ¡lida");
            }

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
                await AplicarCierreAsync(rallyId, resultado.Cierre.DuplaGanadoraId, ctx, TipoCierreRally.PorJuego);

            return resultado;
        }

        public async Task<ResultadoRegistro> CierreDirectoAsync(
            int rallyId, TipoCierreDirecto tipo, int? duplaGanadoraId = null,
            DetalleSaque? detalleSaque = null)
        {
            var ctx = await ArmarContextoAsync(rallyId);
            var resultado = _motor.CierreDirecto(ctx, tipo, duplaGanadoraId);
            TipoCierreRally tipoCierre = tipo switch
            {
                TipoCierreDirecto.Ace          => TipoCierreRally.Ace,
                TipoCierreDirecto.ErrorSaque   => TipoCierreRally.ErrorSaque,
                TipoCierreDirecto.ErrorVario   => TipoCierreRally.ErrorVario,
                TipoCierreDirecto.CierreRapido => TipoCierreRally.CierreRapido,
                _ => throw new ArgumentOutOfRangeException(nameof(tipo))
            };

            if ((tipo == TipoCierreDirecto.Ace || tipo == TipoCierreDirecto.ErrorSaque)
                && detalleSaque != null)
            {
                var calidad = tipo == TipoCierreDirecto.Ace
                    ? Calidad.DoblePositivo : Calidad.DobleNegativo;
                db.Acciones.Add(new Accion
                {
                    RallyId      = rallyId,
                    Secuencia    = 1,
                    JugadorId    = _motor.QuienSaca(ctx),
                    Fundamento   = Fundamento.Saque,
                    Calidad      = calidad,
                    Complejo     = Complejo.K2,
                    EsDe2da      = false,
                    EsRejuego    = false,
                    FechaHora    = DateTime.UtcNow,
                    DetalleSaque = detalleSaque
                });
            }

            await AplicarCierreAsync(rallyId, resultado.Cierre!.DuplaGanadoraId, ctx, tipoCierre);
            return resultado;
        }

        public async Task DegradePrimerContactoAsync(int rallyId)
        {
            var ctx = await ArmarContextoAsync(rallyId);

            var primerContacto = ctx.AccionesRallyActual
                .LastOrDefault(a => a.Fundamento == Fundamento.Recepcion
                                 || a.Fundamento == Fundamento.Defensa);

            if (primerContacto == null)
                throw new InvalidOperationException("No hay recepciÃ³n o defensa en el rally");

            // Degradar si la calidad actual es mejor que Negativo
            bool degradar = !primerContacto.Calidad.HasValue
                         || (int)primerContacto.Calidad.Value > (int)Calidad.Negativo;

            if (degradar)
            {
                var accionEntity = (await db.Acciones.FindAsync(primerContacto.Id))!;
                accionEntity.Calidad = Calidad.Negativo;

                // Recalcular derivaciÃ³n del saque (combinada: Recepcionâˆ’ â†’ Saque+)
                if (primerContacto.Fundamento == Fundamento.Recepcion)
                {
                    var saque = await db.Acciones
                        .Where(a => a.RallyId == rallyId && a.Fundamento == Fundamento.Saque)
                        .FirstOrDefaultAsync();
                    if (saque != null)
                        saque.Calidad = Calidad.Positivo;
                }
                await db.SaveChangesAsync();
            }

            // Recepcion: Regla 3 seguirÃ­a sugiriendo receptor ataca.
            // Insertar marcador FreeBall (sin carga estadÃ­stica, Calidad null) del compaÃ±ero
            // para que Regla 14 garantice rival ataca K2. En Case A el armado NO fue malo.
            // Defensa: Defensa Negativo ya dispara Regla 11 â†’ rival ataca. Sin marcador extra.
            if (primerContacto.Fundamento == Fundamento.Recepcion)
            {
                var duplaEntry = ctx.JugadoresPorDupla
                    .First(kvp => kvp.Value.Any(j => j.JugadorId == primerContacto.JugadorId));
                int companeroId = duplaEntry.Value
                    .First(j => j.JugadorId != primerContacto.JugadorId)
                    .JugadorId;

                db.Acciones.Add(new Accion
                {
                    RallyId    = rallyId,
                    Secuencia  = ctx.AccionesRallyActual.Count + 1,
                    JugadorId  = companeroId,
                    Fundamento = Fundamento.FreeBall,
                    Calidad    = null,
                    Complejo   = Complejo.K2,
                    EsDe2da    = false,
                    EsRejuego  = false,
                    FechaHora  = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }
        }

        public async Task CerrarSetAsync(int setEnVivoId, int duplaGanadoraId)
        {
            var set = await db.SetsEnVivo
                .Include(s => s.PartidoEnVivo)
                .FirstOrDefaultAsync(s => s.Id == setEnVivoId)
                ?? throw new InvalidOperationException($"Set {setEnVivoId} no encontrado");

            var partido = set.PartidoEnVivo!;
            if (duplaGanadoraId != partido.Dupla1Id && duplaGanadoraId != partido.Dupla2Id)
                throw new ArgumentException($"Dupla {duplaGanadoraId} no pertenece al partido");

            set.DuplaGanadoraId = duplaGanadoraId;
            await db.SaveChangesAsync();
        }

        public async Task DeshacerUltimaAccionAsync(int rallyId)
        {
            var ctx = await ArmarContextoAsync(rallyId);
            var rally = await db.Rallies.FindAsync(rallyId)
                ?? throw new InvalidOperationException($"Rally {rallyId} no encontrado");

            bool rallyCerrado = rally.DuplaGanadoraId != null;
            bool sinAcciones  = ctx.AccionesRallyActual.Count == 0;

            // Rally cerrado sin acciones (Ace/ErrorSaque/CierreRapido) â†’ solo reabrir
            if (rallyCerrado && sinAcciones)
            {
                if (rally.DuplaGanadoraId == ctx.Dupla1Id) rally.MarcadorDupla1--;
                else                                        rally.MarcadorDupla2--;
                rally.DuplaGanadoraId = null;
                rally.TipoCierre      = null;
                await db.SaveChangesAsync();
                return;
            }

            // Rally abierto sin acciones â†’ excepciÃ³n
            if (sinAcciones)
                throw new InvalidOperationException("El rally no tiene acciones para deshacer");

            var ultima = ctx.AccionesRallyActual[^1];

            // Deshacer derivaciÃ³n si aplica
            if (ultima.Calidad.HasValue)
            {
                var combinada = ctx.Combinadas.FirstOrDefault(c =>
                    c.FundamentoCargado == ultima.Fundamento &&
                    c.CalidadCargada    == ultima.Calidad.Value &&
                    c.CalidadDerivada   != null);

                if (combinada != null)
                {
                    var target = await db.Acciones
                        .Where(a => a.RallyId    == rallyId
                                 && a.Fundamento == combinada.FundamentoDerivado
                                 && a.Calidad    == combinada.CalidadDerivada)
                        .OrderByDescending(a => a.Secuencia)
                        .FirstOrDefaultAsync();

                    if (target != null)
                        target.Calidad = null;
                }
            }

            // Reabrir rally si estaba cerrado
            if (rallyCerrado)
            {
                if (rally.DuplaGanadoraId == ctx.Dupla1Id) rally.MarcadorDupla1--;
                else                                        rally.MarcadorDupla2--;
                rally.DuplaGanadoraId = null;
                rally.TipoCierre      = null;
            }

            var accionParaEliminar = await db.Acciones.FindAsync(ultima.Id)
                ?? throw new InvalidOperationException($"AcciÃ³n {ultima.Id} no encontrada");
            db.Acciones.Remove(accionParaEliminar);

            await db.SaveChangesAsync();
        }

        public async Task<EstadoRallyData> ObtenerEstadoRallyAsync(int rallyId)
        {
            var ctx   = await ArmarContextoAsync(rallyId);
            var rally = (await db.Rallies.FindAsync(rallyId))!;

            SugerenciaPaso? sugerencia = null;
            string? advertenciaEstado = null;
            if (rally.DuplaGanadoraId == null)
            {
                try { sugerencia = _motor.SugerirProximoPaso(ctx); }
                catch (Exception ex) { advertenciaEstado = ex.Message; }
            }

            var setsGanadores = await db.SetsEnVivo
                .Where(s => s.PartidoEnVivoId == rally.SetEnVivo!.PartidoEnVivoId
                         && s.DuplaGanadoraId != null)
                .Select(s => s.DuplaGanadoraId!.Value)
                .ToListAsync();

            int setsGanadosD1 = setsGanadores.Count(id => id == ctx.Dupla1Id);
            int setsGanadosD2 = setsGanadores.Count(id => id == ctx.Dupla2Id);

            var marcador = _motor.EvaluarMarcador(
                ctx.Dupla1Id, ctx.Dupla2Id,
                rally.MarcadorDupla1, rally.MarcadorDupla2,
                rally.SetEnVivo!.NumeroSet,
                setsGanadosD1, setsGanadosD2);

            return new EstadoRallyData(rally, ctx.AccionesRallyActual, sugerencia, marcador, ctx.NombresJugadores, advertenciaEstado);
        }

        private async Task AplicarCierreAsync(int rallyId, int duplaGanadoraId, ContextoRally ctx, TipoCierreRally tipoCierre)
        {
            var rally = await db.Rallies.FindAsync(rallyId)
                ?? throw new InvalidOperationException($"Rally {rallyId} no encontrado");

            rally.DuplaGanadoraId  = duplaGanadoraId;
            rally.TipoCierre       = tipoCierre;
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
                Combinadas                     = combinadas,
                NombresJugadores               = new Dictionary<int, string>
                {
                    [dupla1.Jugador1!.Id] = dupla1.Jugador1.NombreCompleto,
                    [dupla1.Jugador2!.Id] = dupla1.Jugador2.NombreCompleto,
                    [dupla2.Jugador1!.Id] = dupla2.Jugador1.NombreCompleto,
                    [dupla2.Jugador2!.Id] = dupla2.Jugador2.NombreCompleto,
                }
            };
        }
    }
}
```

## Services\EnVivo\ContextoRally.cs
```csharp
using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public class ContextoRally
    {
        public required int Dupla1Id { get; init; }
        public required int Dupla2Id { get; init; }
        public required IReadOnlyDictionary<int, IReadOnlyList<JugadorEnCancha>> JugadoresPorDupla { get; init; }
        public required int SacadorInicialDupla1JugadorId { get; init; }
        public required int SacadorInicialDupla2JugadorId { get; init; }
        public required int DuplaQueSacaPrimeroId { get; init; }
        public required IReadOnlyList<int> GanadoresRalliesPrevios { get; init; }
        public required IReadOnlyList<Accion> AccionesRallyActual { get; init; }
        public required IReadOnlyList<ModificadorCombinada> Combinadas { get; init; }
        public IReadOnlyDictionary<int, string> NombresJugadores { get; init; } = new Dictionary<int, string>();
    }
}
```

## Services\EnVivo\EstadoRallyData.cs
```csharp
using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public record EstadoRallyData(
        Rally Rally,
        IReadOnlyList<Accion> Acciones,
        SugerenciaPaso? Sugerencia,
        ResultadoMarcador Marcador,
        IReadOnlyDictionary<int, string> NombresJugadores,
        string? AdvertenciaEstado = null
    );
}
```

## Services\EnVivo\InferenciaAtaque.cs
```csharp
using SandStats.Models;
using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public static class InferenciaAtaque
    {
        public static TipoAcciones Inferir(
            string golpe, TipoLado lado, ZonaCancha zona, RolJugador rol)
        {
            int n = (int)zona;
            if (golpe == "Spike")
                return Enum.Parse<TipoAcciones>("Atq" + n);
            return EsLinea(lado, zona, rol)
                ? Enum.Parse<TipoAcciones>("Tl" + n)
                : Enum.Parse<TipoAcciones>("Td" + n);
        }

        private static bool EsLinea(TipoLado lado, ZonaCancha zona, RolJugador rol)
        {
            bool esRol4 = rol == RolJugador.Rol4;
            int n = (int)zona;
            // Tabla extraÃ­da de CargarAtaques.cshtml.cs (pÃ¡gina legacy sin modificar)
            int[] tl = (lado, esRol4) switch
            {
                (TipoLado.Bueno, true)  => [1, 9, 2],
                (TipoLado.Atras, true)  => [5, 7, 4],
                (TipoLado.Medio, true)  => [1, 9, 2],
                (TipoLado.Bueno, false) => [5, 7, 4],   // Rol2 Bueno = Rol4 AtrÃ¡s
                (TipoLado.Atras, false) => [1, 9, 2],   // Rol2 AtrÃ¡s = Rol4 Bueno
                (TipoLado.Medio, false) => [5, 7, 4],
                _                       => []
            };
            return tl.Contains(n);
        }
    }
}
```

## Services\EnVivo\JugadorEnCancha.cs
```csharp
using SandStats.Models;

namespace SandStats.Services.EnVivo
{
    public record JugadorEnCancha(int JugadorId, PosicionJugador Posicion);
}
```

## Services\EnVivo\MotorRally.cs
```csharp
using SandStats.Models;
using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public class MotorRally
    {
        public int QuienSaca(ContextoRally ctx)
        {
            int duplaServante = ctx.GanadoresRalliesPrevios.Count == 0
                ? ctx.DuplaQueSacaPrimeroId
                : ctx.GanadoresRalliesPrevios[^1];

            var adquisiciones = new Dictionary<int, int>
            {
                [ctx.Dupla1Id] = 0,
                [ctx.Dupla2Id] = 0
            };
            adquisiciones[ctx.DuplaQueSacaPrimeroId] = 1;

            int sacadorActual = ctx.DuplaQueSacaPrimeroId;
            foreach (int ganador in ctx.GanadoresRalliesPrevios)
            {
                if (ganador != sacadorActual)
                {
                    sacadorActual = ganador;
                    adquisiciones[ganador]++;
                }
            }

            int sacadorInicial = duplaServante == ctx.Dupla1Id
                ? ctx.SacadorInicialDupla1JugadorId
                : ctx.SacadorInicialDupla2JugadorId;

            if (adquisiciones[duplaServante] % 2 == 1)
                return sacadorInicial;

            return ctx.JugadoresPorDupla[duplaServante]
                .First(j => j.JugadorId != sacadorInicial)
                .JugadorId;
        }

        public ResultadoRegistro Registrar(ContextoRally ctx, CargaAccion carga)
        {
            if (carga.Fundamento is Fundamento.Recepcion or Fundamento.Bloqueo or Fundamento.Defensa)
            {
                if (!carga.Calidad.HasValue ||
                    !ctx.Combinadas.Any(c => c.FundamentoCargado == carga.Fundamento
                                          && c.CalidadCargada    == carga.Calidad.Value))
                    throw new ArgumentException(
                        $"{carga.Fundamento} con calidad {carga.Calidad?.ToString() ?? "null"} " +
                        $"no es una combinaciÃ³n vÃ¡lida");
            }

            Derivacion? derivacion = null;
            if (carga.Calidad.HasValue)
            {
                var combinada = ctx.Combinadas.FirstOrDefault(c =>
                    c.FundamentoCargado == carga.Fundamento &&
                    c.CalidadCargada == carga.Calidad.Value &&
                    c.CalidadDerivada != null);
                if (combinada != null)
                    derivacion = new Derivacion(combinada.FundamentoDerivado, combinada.CalidadDerivada!.Value);
            }

            Cierre? cierre = null;
            int duplaJugador = DuplaDeJugador(ctx, carga.JugadorId);
            int duplaServante = DuplaServante(ctx);

            if (carga.Fundamento == Fundamento.Recepcion && carga.Calidad == Calidad.DobleNegativo)
                cierre = new Cierre(duplaServante);
            else if (carga.Fundamento == Fundamento.Ataque && carga.Calidad == Calidad.DoblePositivo)
                cierre = new Cierre(duplaJugador);
            else if (carga.Fundamento == Fundamento.Ataque && carga.Calidad == Calidad.DobleNegativo)
                cierre = new Cierre(RivalDe(ctx, duplaJugador));
            else if (carga.Fundamento == Fundamento.Bloqueo && carga.Calidad == Calidad.DoblePositivo)
                cierre = new Cierre(duplaJugador);
            else if (carga.Fundamento == Fundamento.Bloqueo && carga.Calidad == Calidad.DobleNegativo)
                cierre = new Cierre(RivalDe(ctx, duplaJugador));
            else if (carga.Fundamento == Fundamento.Defensa && carga.Calidad == Calidad.DobleNegativo)
                cierre = new Cierre(RivalDe(ctx, duplaJugador));

            return new ResultadoRegistro(derivacion, cierre);
        }

        public ResultadoRegistro CierreDirecto(ContextoRally ctx, TipoCierreDirecto tipo, int? duplaGanadoraId = null)
        {
            int duplaServante = DuplaServante(ctx);
            int ganadora = tipo switch
            {
                TipoCierreDirecto.Ace         => duplaServante,
                TipoCierreDirecto.ErrorSaque  => RivalDe(ctx, duplaServante),
                TipoCierreDirecto.ErrorVario  => duplaGanadoraId ?? throw new ArgumentNullException(nameof(duplaGanadoraId)),
                TipoCierreDirecto.CierreRapido => duplaGanadoraId ?? throw new ArgumentNullException(nameof(duplaGanadoraId)),
                _ => throw new ArgumentOutOfRangeException(nameof(tipo))
            };
            return new ResultadoRegistro(null, new Cierre(ganadora));
        }

        public SugerenciaPaso SugerirProximoPaso(ContextoRally ctx)
        {
            // Regla 1: rally vacÃ­o â†’ saque
            if (ctx.AccionesRallyActual.Count == 0)
            {
                int sacador = QuienSaca(ctx);
                return new SugerenciaPaso(
                    Opciones: [new OpcionPaso(Fundamento.Saque, sacador)],
                    DuplaId: DuplaServante(ctx),
                    Complejo: Complejo.K2,
                    PermiteDe2da: false,
                    JugadorDe2daId: null,
                    EsRejuego: false
                );
            }

            var ultima = ctx.AccionesRallyActual[^1];
            bool yaHuboAtaque = ctx.AccionesRallyActual.Any(a => a.Fundamento == Fundamento.Ataque);

            // Regla 2: saque sin calidad â†’ recepcion de la dupla rival
            if (ultima.Fundamento == Fundamento.Saque && ultima.Calidad == null)
            {
                int duplaReceptora = RivalDe(ctx, DuplaDeJugador(ctx, ultima.JugadorId));
                return new SugerenciaPaso(
                    Opciones: [new OpcionPaso(Fundamento.Recepcion, null)],
                    DuplaId: duplaReceptora,
                    Complejo: Complejo.K1,
                    PermiteDe2da: false,
                    JugadorDe2daId: null,
                    EsRejuego: false
                );
            }

            // Reglas 3 y 4: recepcion
            if (ultima.Fundamento == Fundamento.Recepcion)
            {
                int duplaReceptora = DuplaDeJugador(ctx, ultima.JugadorId);

                // Regla 4: vendida â†’ dupla sacadora ataca, K2
                if (ultima.Calidad == Calidad.Slash)
                {
                    return new SugerenciaPaso(
                        Opciones: [new OpcionPaso(Fundamento.Ataque, null)],
                        DuplaId: DuplaServante(ctx),
                        Complejo: Complejo.K2,
                        PermiteDe2da: false,
                        JugadorDe2daId: null,
                        EsRejuego: false
                    );
                }

                // Regla 3: #/+/!/âˆ’ â†’ dupla receptora ataca
                // Regla 12 (transversal): si ya hubo un ataque en el rally, K2 en vez de K1
                Complejo complejo = yaHuboAtaque ? Complejo.K2 : Complejo.K1;
                bool permiteDe2da = !yaHuboAtaque;
                int? jugadorDe2daId = permiteDe2da ? Companero(ctx, ultima.JugadorId) : null;
                return new SugerenciaPaso(
                    Opciones: [new OpcionPaso(Fundamento.Ataque, ultima.JugadorId)],
                    DuplaId: duplaReceptora,
                    Complejo: complejo,
                    PermiteDe2da: permiteDe2da,
                    JugadorDe2daId: jugadorDe2daId,
                    EsRejuego: false
                );
            }

            // Regla 5: ataque sin calidad â†’ bloqueo y defensa del rival
            if (ultima.Fundamento == Fundamento.Ataque && ultima.Calidad == null)
            {
                int duplaRival = RivalDe(ctx, DuplaDeJugador(ctx, ultima.JugadorId));
                var jugadoresRival = ctx.JugadoresPorDupla[duplaRival];
                int bloqueadorId = jugadoresRival.First(j => j.Posicion == PosicionJugador.Bloqueador).JugadorId;
                int defensorId   = jugadoresRival.First(j => j.Posicion == PosicionJugador.Defensor).JugadorId;
                return new SugerenciaPaso(
                    Opciones: [
                        new OpcionPaso(Fundamento.Bloqueo, bloqueadorId, CalidadesDesde(ctx, Fundamento.Bloqueo)),
                        new OpcionPaso(Fundamento.Defensa, defensorId,   CalidadesDesde(ctx, Fundamento.Defensa))],
                    DuplaId: duplaRival,
                    Complejo: Complejo.K2,
                    PermiteDe2da: false,
                    JugadorDe2daId: null,
                    EsRejuego: false
                );
            }

            // Reglas 6, 7, 8: bloqueo
            if (ultima.Fundamento == Fundamento.Bloqueo)
            {
                // Regla 6: bloqueo + â†’ dupla del bloqueador ataca, K2
                if (ultima.Calidad == Calidad.Positivo)
                {
                    return new SugerenciaPaso(
                        Opciones: [new OpcionPaso(Fundamento.Ataque, null)],
                        DuplaId: DuplaDeJugador(ctx, ultima.JugadorId),
                        Complejo: Complejo.K2,
                        PermiteDe2da: false,
                        JugadorDe2daId: null,
                        EsRejuego: false
                    );
                }

                // Regla 7: bloqueo âˆ’ â†’ mismo atacante previo, K2, permite 2da
                if (ultima.Calidad == Calidad.Negativo)
                {
                    var atacantePrevio = UltimaAccionConFundamento(ctx, Fundamento.Ataque);
                    return new SugerenciaPaso(
                        Opciones: [new OpcionPaso(Fundamento.Ataque, atacantePrevio.JugadorId)],
                        DuplaId: DuplaDeJugador(ctx, atacantePrevio.JugadorId),
                        Complejo: Complejo.K2,
                        PermiteDe2da: true,
                        JugadorDe2daId: Companero(ctx, atacantePrevio.JugadorId),
                        EsRejuego: false
                    );
                }

                // Regla 8: bloqueo / â†’ mismo atacante previo, K2, rejuego, permite 2da
                if (ultima.Calidad == Calidad.Slash)
                {
                    var atacantePrevio = UltimaAccionConFundamento(ctx, Fundamento.Ataque);
                    return new SugerenciaPaso(
                        Opciones: [new OpcionPaso(Fundamento.Ataque, atacantePrevio.JugadorId)],
                        DuplaId: DuplaDeJugador(ctx, atacantePrevio.JugadorId),
                        Complejo: Complejo.K2,
                        PermiteDe2da: true,
                        JugadorDe2daId: Companero(ctx, atacantePrevio.JugadorId),
                        EsRejuego: true
                    );
                }
            }

            // Reglas 9, 10, 11: defensa
            if (ultima.Fundamento == Fundamento.Defensa)
            {
                // Regla 9: defensa #/+ â†’ dupla del defensor ataca, K2, permite 2da
                if (ultima.Calidad == Calidad.DoblePositivo || ultima.Calidad == Calidad.Positivo)
                {
                    int companero = Companero(ctx, ultima.JugadorId);
                    return new SugerenciaPaso(
                        Opciones: [new OpcionPaso(Fundamento.Ataque, ultima.JugadorId)],
                        DuplaId: DuplaDeJugador(ctx, ultima.JugadorId),
                        Complejo: Complejo.K2,
                        PermiteDe2da: true,
                        JugadorDe2daId: companero,
                        EsRejuego: false
                    );
                }

                // Regla 10: defensa ! (cobertura) â†’ mismo atacante previo, K2, permite 2da
                if (ultima.Calidad == Calidad.Exclamativa)
                {
                    var atacantePrevio = UltimaAccionConFundamento(ctx, Fundamento.Ataque);
                    return new SugerenciaPaso(
                        Opciones: [new OpcionPaso(Fundamento.Ataque, atacantePrevio.JugadorId)],
                        DuplaId: DuplaDeJugador(ctx, atacantePrevio.JugadorId),
                        Complejo: Complejo.K2,
                        PermiteDe2da: true,
                        JugadorDe2daId: Companero(ctx, atacantePrevio.JugadorId),
                        EsRejuego: false
                    );
                }

                // Regla 11: defensa âˆ’/ â†’ atacante previo, K2, permite 2da
                if (ultima.Calidad == Calidad.Negativo || ultima.Calidad == Calidad.Slash)
                {
                    var atacantePrevio = UltimaAccionConFundamento(ctx, Fundamento.Ataque);
                    return new SugerenciaPaso(
                        Opciones: [new OpcionPaso(Fundamento.Ataque, atacantePrevio.JugadorId)],
                        DuplaId: DuplaDeJugador(ctx, atacantePrevio.JugadorId),
                        Complejo: Complejo.K2,
                        PermiteDe2da: true,
                        JugadorDe2daId: Companero(ctx, atacantePrevio.JugadorId),
                        EsRejuego: false
                    );
                }
            }

            // Regla 13: armado âˆ’ â†’ rival ataca, free ball K2
            if (ultima.Fundamento == Fundamento.Armado && ultima.Calidad == Calidad.Negativo)
            {
                return new SugerenciaPaso(
                    Opciones: [new OpcionPaso(Fundamento.Ataque, null)],
                    DuplaId: RivalDe(ctx, DuplaDeJugador(ctx, ultima.JugadorId)),
                    Complejo: Complejo.K2,
                    PermiteDe2da: false,
                    JugadorDe2daId: null,
                    EsRejuego: false
                );
            }

            // Regla 14: free ball (marcador de posesiÃ³n, Calidad null) â†’ rival ataca K2
            if (ultima.Fundamento == Fundamento.FreeBall)
            {
                return new SugerenciaPaso(
                    Opciones: [new OpcionPaso(Fundamento.Ataque, null)],
                    DuplaId: RivalDe(ctx, DuplaDeJugador(ctx, ultima.JugadorId)),
                    Complejo: Complejo.K2,
                    PermiteDe2da: false,
                    JugadorDe2daId: null,
                    EsRejuego: false
                );
            }

            throw new InvalidOperationException($"Estado no manejado: {ultima.Fundamento}/{ultima.Calidad}");
        }

        private static IReadOnlyList<Calidad> CalidadesDesde(ContextoRally ctx, Fundamento f) =>
            ctx.Combinadas
               .Where(c => c.FundamentoCargado == f)
               .Select(c => c.CalidadCargada)
               .Distinct()
               .OrderBy(c => (int)c)
               .ToList();

        private static int Companero(ContextoRally ctx, int jugadorId)
        {
            int dupla = DuplaDeJugador(ctx, jugadorId);
            return ctx.JugadoresPorDupla[dupla].First(j => j.JugadorId != jugadorId).JugadorId;
        }

        private static Accion UltimaAccionConFundamento(ContextoRally ctx, Fundamento f)
            => ctx.AccionesRallyActual.Last(a => a.Fundamento == f);

        public ResultadoMarcador EvaluarMarcador(
            int dupla1Id, int dupla2Id,
            int puntosD1, int puntosD2,
            int numeroSet, int setsGanadosD1, int setsGanadosD2)
        {
            int objetivo = numeroSet <= 2 ? 21 : 15;
            bool setTerminado = (puntosD1 >= objetivo || puntosD2 >= objetivo)
                             && Math.Abs(puntosD1 - puntosD2) >= 2;

            int? ganadorSetId = null;
            bool partidoTerminado = false;

            if (setTerminado)
            {
                bool d1Gano = puntosD1 > puntosD2;
                ganadorSetId = d1Gano ? dupla1Id : dupla2Id;
                partidoTerminado = (d1Gano && setsGanadosD1 + 1 == 2)
                                || (!d1Gano && setsGanadosD2 + 1 == 2);
            }

            int total = puntosD1 + puntosD2;
            int divisor = numeroSet <= 2 ? 7 : 5;
            bool cambioDeLado = !setTerminado && total > 0 && total % divisor == 0;

            return new ResultadoMarcador(setTerminado, partidoTerminado, cambioDeLado, ganadorSetId);
        }

        private static int DuplaDeJugador(ContextoRally ctx, int jugadorId)
        {
            foreach (var (duplaId, jugadores) in ctx.JugadoresPorDupla)
                if (jugadores.Any(j => j.JugadorId == jugadorId))
                    return duplaId;
            throw new InvalidOperationException($"Jugador {jugadorId} no encontrado en JugadoresPorDupla");
        }

        private static int DuplaServante(ContextoRally ctx)
            => ctx.GanadoresRalliesPrevios.Count == 0
                ? ctx.DuplaQueSacaPrimeroId
                : ctx.GanadoresRalliesPrevios[^1];

        private static int RivalDe(ContextoRally ctx, int duplaId)
            => duplaId == ctx.Dupla1Id ? ctx.Dupla2Id : ctx.Dupla1Id;
    }
}
```

## Services\EnVivo\OpcionPaso.cs
```csharp
using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public record OpcionPaso(
        Fundamento Fundamento,
        int? JugadorSugeridoId,
        IReadOnlyList<Calidad>? CalidadesValidas = null);
}
```

## Services\EnVivo\ResultadoMarcador.cs
```csharp
namespace SandStats.Services.EnVivo
{
    public record ResultadoMarcador(
        bool SetTerminado,
        bool PartidoTerminado,
        bool CambioDeLado,
        int? DuplaGanadoraSetId
    );
}
```

## Services\EnVivo\ResultadoRegistro.cs
```csharp
using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public record Derivacion(Fundamento FundamentoDerivado, Calidad CalidadDerivada);
    public record Cierre(int DuplaGanadoraId);
    public record ResultadoRegistro(Derivacion? Derivacion, Cierre? Cierre);
}
```

## Services\EnVivo\SugerenciaPaso.cs
```csharp
using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public record SugerenciaPaso(
        IReadOnlyList<OpcionPaso> Opciones,
        int DuplaId,
        Complejo Complejo,
        bool PermiteDe2da,
        int? JugadorDe2daId,
        bool EsRejuego
    );
}
```

## Services\EnVivo\TipoCierreDirecto.cs
```csharp
namespace SandStats.Services.EnVivo
{
    public enum TipoCierreDirecto
    {
        Ace,
        ErrorSaque,
        ErrorVario,
        CierreRapido
    }
}
```

## Endpoints\EnVivo\Dtos.cs
```csharp
using SandStats.Models;
using SandStats.Models.EnVivo;
using SandStats.Models.SandStats.Models;
using SandStats.Services.EnVivo;

namespace SandStats.Endpoints.EnVivo
{
    // â”€â”€ Requests â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    public record CrearPartidoRequest(int Dupla1Id, int Dupla2Id, string Torneo, DateTime Fecha);

    public record IniciarSetRequest(
        int NumeroSet,
        int SacadorInicialD1Id,
        int SacadorInicialD2Id,
        int DuplaQueSacaPrimeroId);

    // Flat: cubre DetalleSaque, DetalleRecepcion y DetalleAtaque; el handler valida coherencia.
    public record DetalleAccionRequest(
        TipoLado? Lado,
        TipoAcciones? TipoAccion,
        ZonaCancha? ZonaDestino,
        bool? EsVarilla,
        bool? EsEspecial,
        ZonaSaque? ZonaSaque,
        TipoSaque? TipoSaque,
        TipoRecepcion? TipoRecepcion,
        string? GolpeSimplificado);   // "Spike"|"Tip" â€” el servidor infiere TipoAccion

    public record RegistrarAccionRequest(
        Fundamento Fundamento,
        Calidad? Calidad,
        int JugadorId,
        bool EsDe2da,
        DetalleAccionRequest? Detalle);

    public record CierreDirectoRequest(
        TipoCierreDirecto Tipo, int? DuplaGanadoraId,
        ZonaSaque? ZonaSaque, TipoSaque? TipoSaque);

    public record CerrarSetRequest(int DuplaGanadoraId);

    // â”€â”€ Responses â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

    public record PartidoResponse(int Id, int Dupla1Id, int Dupla2Id, string Torneo, DateTime Fecha);

    public record SetIniciadoResponse(int Id, int PartidoId, int NumeroSet);

    public record RallyAbiertoResponse(int Id, int NumeroRally, int MarcadorDupla1, int MarcadorDupla2);

    public record AccionResponse(
        int Secuencia,
        int JugadorId,
        string JugadorNombre,
        Fundamento Fundamento,
        Calidad? Calidad,
        Complejo Complejo,
        bool EsRejuego);

    public record OpcionPasoResponse(
        Fundamento Fundamento,
        int? JugadorSugeridoId,
        string? JugadorNombre,
        IReadOnlyList<string> CalidadesValidas);

    public record SugerenciaPasoResponse(
        IReadOnlyList<OpcionPasoResponse> Opciones,
        int DuplaId,
        Complejo Complejo,
        bool PermiteDe2da,
        int? JugadorDe2daId,
        string? JugadorDe2daNombre,
        bool EsRejuego);

    public record ResultadoMarcadorResponse(
        bool SetTerminado,
        bool PartidoTerminado,
        bool CambioDeLado,
        int? DuplaGanadoraSetId);

    public record EstadoRallyResponse(
        int RallyId,
        int NumeroRally,
        int MarcadorDupla1,
        int MarcadorDupla2,
        IReadOnlyList<AccionResponse> Acciones,
        SugerenciaPasoResponse? Sugerencia,
        ResultadoMarcadorResponse Marcador,
        bool Cerrado,
        int? DuplaGanadoraId,
        TipoCierreRally? TipoCierre,
        string? AdvertenciaEstado);
}
```

## Endpoints\EnVivo\EnVivoEndpoints.cs
```csharp
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using SandStats.Models.EnVivo;
using SandStats.Models.SandStats.Models;
using SandStats.Services.EnVivo;

namespace SandStats.Endpoints.EnVivo
{
    public static class EnVivoEndpoints
    {
        // CSRF mitigado por SameSite=Lax (decisiÃ³n consciente): la UI es same-origin y el estado
        // mutable queda protegido por la cookie de Identity. Sin antiforgery en el grupo API.
        public static void MapEnVivoEndpoints(this IEndpointRouteBuilder app)
        {
            var g = app.MapGroup("/api/envivo").RequireAuthorization();

            g.MapPost("/partidos", async (CrearPartidoRequest req, CargaEnVivoService svc) =>
            {
                try
                {
                    var p = await svc.CrearPartidoAsync(req.Dupla1Id, req.Dupla2Id, req.Torneo, req.Fecha);
                    return Results.Created($"/api/envivo/partidos/{p.Id}",
                        new PartidoResponse(p.Id, p.Dupla1Id, p.Dupla2Id, p.Torneo, p.Fecha));
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapPost("/partidos/{id:int}/sets", async (int id, IniciarSetRequest req, CargaEnVivoService svc) =>
            {
                try
                {
                    var s = await svc.IniciarSetAsync(
                        id, req.NumeroSet, req.SacadorInicialD1Id, req.SacadorInicialD2Id, req.DuplaQueSacaPrimeroId);
                    return Results.Created($"/api/envivo/sets/{s.Id}",
                        new SetIniciadoResponse(s.Id, s.PartidoEnVivoId, s.NumeroSet));
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapPost("/sets/{id:int}/rallies", async (int id, CargaEnVivoService svc) =>
            {
                try
                {
                    var r = await svc.AbrirRallyAsync(id);
                    return Results.Created($"/api/envivo/rallies/{r.Id}",
                        new RallyAbiertoResponse(r.Id, r.NumeroRally, r.MarcadorDupla1, r.MarcadorDupla2));
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapPost("/rallies/{id:int}/acciones", async (int id, RegistrarAccionRequest req, CargaEnVivoService svc, ApplicationDbContext db) =>
            {
                try
                {
                    object? detalle;
                    if (req.Fundamento == Fundamento.Ataque
                        && req.Detalle?.GolpeSimplificado != null)
                    {
                        var jug = await db.Jugadores.FindAsync(req.JugadorId)
                            ?? throw new ArgumentException($"Jugador {req.JugadorId} no encontrado");
                        var lado = req.Detalle.Lado
                            ?? throw new ArgumentException("Lado requerido para Ataque");
                        var zona = req.Detalle.ZonaDestino
                            ?? throw new ArgumentException("ZonaDestino requerida para Ataque");
                        detalle = new DetalleAtaque
                        {
                            Lado        = lado,
                            TipoAccion  = InferenciaAtaque.Inferir(req.Detalle.GolpeSimplificado, lado, zona, jug.RolPrincipal),
                            ZonaDestino = zona,
                            EsVarilla   = req.Detalle.EsVarilla  ?? false,
                            EsEspecial  = false
                        };
                    }
                    else
                    {
                        detalle = MapDetalle(req.Fundamento, req.Detalle);
                    }
                    await svc.RegistrarAccionAsync(id, new CargaAccion(req.Fundamento, req.Calidad, req.JugadorId, req.EsDe2da), detalle);
                    return Results.Ok(MapEstado(await svc.ObtenerEstadoRallyAsync(id)));
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapPost("/rallies/{id:int}/degradar-primer-contacto", async (int id, CargaEnVivoService svc) =>
            {
                try
                {
                    await svc.DegradePrimerContactoAsync(id);
                    return Results.Ok(MapEstado(await svc.ObtenerEstadoRallyAsync(id)));
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapPost("/rallies/{id:int}/cierre-directo", async (int id, CierreDirectoRequest req, CargaEnVivoService svc) =>
            {
                try
                {
                    DetalleSaque? detalleSaque = null;
                    if ((req.Tipo == TipoCierreDirecto.Ace || req.Tipo == TipoCierreDirecto.ErrorSaque)
                        && req.ZonaSaque.HasValue && req.TipoSaque.HasValue)
                    {
                        detalleSaque = new DetalleSaque
                        {
                            ZonaSaque = req.ZonaSaque.Value,
                            TipoSaque = req.TipoSaque.Value
                        };
                    }
                    await svc.CierreDirectoAsync(id, req.Tipo, req.DuplaGanadoraId, detalleSaque);
                    return Results.Ok(MapEstado(await svc.ObtenerEstadoRallyAsync(id)));
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapPost("/rallies/{id:int}/deshacer", async (int id, CargaEnVivoService svc) =>
            {
                try
                {
                    await svc.DeshacerUltimaAccionAsync(id);
                    return Results.Ok(MapEstado(await svc.ObtenerEstadoRallyAsync(id)));
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapPost("/sets/{id:int}/cerrar", async (int id, CerrarSetRequest req, CargaEnVivoService svc) =>
            {
                try
                {
                    await svc.CerrarSetAsync(id, req.DuplaGanadoraId);
                    return Results.NoContent();
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapGet("/sets/{id:int}/estado", async (int id, CargaEnVivoService svc, ApplicationDbContext db) =>
            {
                try
                {
                    var setExiste = await db.SetsEnVivo.AnyAsync(s => s.Id == id);
                    if (!setExiste)
                        return Results.Problem($"Set {id} no encontrado", statusCode: 404);

                    var rallyId = await db.Rallies
                        .Where(r => r.SetEnVivoId == id)
                        .OrderByDescending(r => r.NumeroRally)
                        .Select(r => (int?)r.Id)
                        .FirstOrDefaultAsync();

                    if (rallyId == null)
                    {
                        // Set reciÃ©n iniciado sin rallies: estado vacÃ­o con cerrado=true para mostrar "Abrir rally"
                        return Results.Ok(new EstadoRallyResponse(
                            0, 0, 0, 0,
                            Array.Empty<AccionResponse>(),
                            null,
                            new ResultadoMarcadorResponse(false, false, false, null),
                            Cerrado: true,
                            null, null, null));
                    }

                    return Results.Ok(MapEstado(await svc.ObtenerEstadoRallyAsync(rallyId.Value)));
                }
                catch (Exception ex) { return MapError(ex); }
            });
        }

        private static IResult MapError(Exception ex) => ex switch
        {
            ArgumentException =>
                Results.Problem(ex.Message, statusCode: 400),
            InvalidOperationException e when e.Message.Contains("no encontrado") =>
                Results.Problem(ex.Message, statusCode: 404),
            _ =>
                Results.Problem(ex.Message, statusCode: 400)
        };

        private static object? MapDetalle(Fundamento fundamento, DetalleAccionRequest? req)
        {
            if (req == null) return null;
            return fundamento switch
            {
                Fundamento.Saque => new DetalleSaque
                {
                    ZonaSaque = req.ZonaSaque  ?? throw new ArgumentException("ZonaSaque requerida para Saque"),
                    TipoSaque = req.TipoSaque  ?? throw new ArgumentException("TipoSaque requerido para Saque")
                },
                Fundamento.Recepcion => new DetalleRecepcion
                {
                    TipoRecepcion = req.TipoRecepcion ?? throw new ArgumentException("TipoRecepcion requerida para Recepcion")
                },
                Fundamento.Ataque => new DetalleAtaque
                {
                    Lado        = req.Lado        ?? throw new ArgumentException("Lado requerido para Ataque"),
                    TipoAccion  = req.TipoAccion  ?? throw new ArgumentException("TipoAccion requerido para Ataque"),
                    ZonaDestino = req.ZonaDestino ?? throw new ArgumentException("ZonaDestino requerida para Ataque"),
                    EsVarilla   = req.EsVarilla  ?? false,
                    EsEspecial  = req.EsEspecial ?? false
                },
                _ => throw new ArgumentException($"El fundamento {fundamento} no admite detalle")
            };
        }

        private static EstadoRallyResponse MapEstado(EstadoRallyData estado)
        {
            var rally  = estado.Rally;
            var m      = estado.Marcador;
            var nombre = (int id) => estado.NombresJugadores.GetValueOrDefault(id, "");

            SugerenciaPasoResponse? sugerencia = null;
            if (estado.Sugerencia != null)
            {
                var s = estado.Sugerencia;
                sugerencia = new SugerenciaPasoResponse(
                    s.Opciones.Select(o => new OpcionPasoResponse(
                        o.Fundamento,
                        o.JugadorSugeridoId,
                        o.JugadorSugeridoId.HasValue ? nombre(o.JugadorSugeridoId.Value) : null,
                        o.CalidadesValidas?.Select(c => c.ToString()).ToList() ?? [])).ToList(),
                    s.DuplaId, s.Complejo, s.PermiteDe2da,
                    s.JugadorDe2daId,
                    s.JugadorDe2daId.HasValue ? nombre(s.JugadorDe2daId.Value) : null,
                    s.EsRejuego);
            }

            return new EstadoRallyResponse(
                rally.Id, rally.NumeroRally,
                rally.MarcadorDupla1, rally.MarcadorDupla2,
                estado.Acciones
                    .Select(a => new AccionResponse(a.Secuencia, a.JugadorId, nombre(a.JugadorId), a.Fundamento, a.Calidad, a.Complejo, a.EsRejuego))
                    .ToList(),
                sugerencia,
                new ResultadoMarcadorResponse(m.SetTerminado, m.PartidoTerminado, m.CambioDeLado, m.DuplaGanadoraSetId),
                rally.DuplaGanadoraId != null,
                rally.DuplaGanadoraId,
                rally.TipoCierre,
                estado.AdvertenciaEstado);
        }
    }
}
```

## SandStats.Tests\CargaEnVivoServiceTests.cs
```csharp
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using SandStats.Models.EnVivo;
using SandStats.Models.SandStats.Models;
using SandStats.Services.EnVivo;
using Xunit;

namespace SandStats.Tests
{
    public class CargaEnVivoServiceTests : IDisposable
    {
        private readonly SqliteConnection _conn;
        private readonly ApplicationDbContext _db;
        private readonly CargaEnVivoService _svc;

        // Entidades de master data compartidas entre tests de esta clase
        private readonly Dupla _d1, _d2;
        private readonly Jugador _j1, _j2, _j3, _j4;

        public CargaEnVivoServiceTests()
        {
            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();

            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_conn)
                .Options;

            _db = new ApplicationDbContext(opts);
            _db.Database.EnsureCreated(); // schema + seed ModificadoresCombinadas

            _svc = new CargaEnVivoService(_db);

            // Seed: 4 jugadores, 2 duplas
            _j1 = new Jugador { Nombre = "J1", Apellido = "D1", Posicion = PosicionJugador.Bloqueador, RolPrincipal = RolJugador.Rol4 };
            _j2 = new Jugador { Nombre = "J2", Apellido = "D1", Posicion = PosicionJugador.Defensor,  RolPrincipal = RolJugador.Rol2 };
            _j3 = new Jugador { Nombre = "J3", Apellido = "D2", Posicion = PosicionJugador.Bloqueador, RolPrincipal = RolJugador.Rol4 };
            _j4 = new Jugador { Nombre = "J4", Apellido = "D2", Posicion = PosicionJugador.Defensor,  RolPrincipal = RolJugador.Rol2 };
            _db.Jugadores.AddRange(_j1, _j2, _j3, _j4);
            _db.SaveChanges();

            _d1 = new Dupla { Jugador1Id = _j1.Id, Jugador2Id = _j2.Id };
            _d2 = new Dupla { Jugador1Id = _j3.Id, Jugador2Id = _j4.Id };
            _db.Duplas.AddRange(_d1, _d2);
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
            _conn.Close();
        }

        private async Task<(PartidoEnVivo partido, SetEnVivo set)> CrearPartidoYSet()
        {
            var partido = await _svc.CrearPartidoAsync(
                _d1.Id, _d2.Id, "Torneo Test", DateTime.UtcNow);
            var set = await _svc.IniciarSetAsync(
                partido.Id, numeroSet: 1,
                sacadorInicialD1: _j1.Id,
                sacadorInicialD2: _j3.Id,
                duplaQueSacaPrimeroId: _d1.Id);
            return (partido, set);
        }

        // â”€â”€ Test 1: rally completo con derivaciones y cierre â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task RallyCompleto_DerivacionesYCierre_MarcadorCorrecto()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            // 1. Saque sin calidad (J1/D1)
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);

            // 2. Recepcion Positivo (J3/D2) â†’ deriva Saqueâ†’Negativo
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, _j3.Id, false), null);

            // 3. Ataque sin calidad (J3/D2) con DetalleAtaque
            var detalleAtaque = new DetalleAtaque
            {
                Lado = TipoLado.Bueno,
                TipoAccion = TipoAcciones.Atq1,
                ZonaDestino = ZonaCancha.Zona1
            };
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Ataque, null, _j3.Id, false), detalleAtaque);

            // 4. Bloqueo DobleNegativo (J1/D1) â†’ deriva Ataqueâ†’DoblePositivo, cierra D2
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Bloqueo, Calidad.DobleNegativo, _j1.Id, false), null);

            // Recargar desde DB
            var acciones = await _db.Acciones
                .Include(a => a.DetalleAtaque)
                .Where(a => a.RallyId == rally.Id)
                .OrderBy(a => a.Secuencia)
                .ToListAsync();

            var rallyActualizado = await _db.Rallies.FindAsync(rally.Id);

            // DerivaciÃ³n 1: saque tiene Calidad=Negativo
            Assert.Equal(Calidad.Negativo, acciones[0].Calidad);

            // DerivaciÃ³n 2: ataque tiene Calidad=DoblePositivo
            Assert.Equal(Calidad.DoblePositivo, acciones[2].Calidad);

            // DetalleAtaque persistido
            Assert.NotNull(acciones[2].DetalleAtaque);

            // Cierre: D2 ganÃ³ (Bloqueo DobleNegativo de J1/D1 â†’ rival = D2)
            Assert.Equal(_d2.Id, rallyActualizado!.DuplaGanadoraId);
            Assert.Equal(0, rallyActualizado.MarcadorDupla1);
            Assert.Equal(1, rallyActualizado.MarcadorDupla2);
        }

        // â”€â”€ Test 2b: cierre directo Ace con DetalleSaque â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task CierreDirectoAce_ConDetalle_PersisteSaqueDoblePositivo()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            var detalle = new DetalleSaque { ZonaSaque = ZonaSaque.Zona1, TipoSaque = TipoSaque.Potencia };
            await _svc.CierreDirectoAsync(rally.Id, TipoCierreDirecto.Ace, null, detalle);

            var acciones = await _db.Acciones.Include(a => a.DetalleSaque)
                .Where(a => a.RallyId == rally.Id).ToListAsync();
            var rallyActualizado = await _db.Rallies.FindAsync(rally.Id);

            Assert.Single(acciones);
            Assert.Equal(Fundamento.Saque, acciones[0].Fundamento);
            Assert.Equal(Calidad.DoblePositivo, acciones[0].Calidad);
            Assert.NotNull(acciones[0].DetalleSaque);
            Assert.Equal(ZonaSaque.Zona1, acciones[0].DetalleSaque!.ZonaSaque);
            Assert.Equal(_d1.Id, rallyActualizado!.DuplaGanadoraId);
            Assert.Equal(1, rallyActualizado.MarcadorDupla1);
        }

        [Fact]
        public async Task CierreDirectoErrorSaque_ConDetalle_PersisteSaqueDobleNegativo()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            var detalle = new DetalleSaque { ZonaSaque = ZonaSaque.Zona6, TipoSaque = TipoSaque.Flotado };
            await _svc.CierreDirectoAsync(rally.Id, TipoCierreDirecto.ErrorSaque, null, detalle);

            var acciones = await _db.Acciones.Include(a => a.DetalleSaque)
                .Where(a => a.RallyId == rally.Id).ToListAsync();
            var rallyActualizado = await _db.Rallies.FindAsync(rally.Id);

            Assert.Single(acciones);
            Assert.Equal(Fundamento.Saque, acciones[0].Fundamento);
            Assert.Equal(Calidad.DobleNegativo, acciones[0].Calidad);
            Assert.NotNull(acciones[0].DetalleSaque);
            Assert.Equal(_d2.Id, rallyActualizado!.DuplaGanadoraId);
            Assert.Equal(1, rallyActualizado.MarcadorDupla2);
        }

        [Fact]
        public async Task CierreDirectoAce_ConDetalle_Deshacer_EliminaAccionYReabre()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            var detalle = new DetalleSaque { ZonaSaque = ZonaSaque.Zona1, TipoSaque = TipoSaque.Potencia };
            await _svc.CierreDirectoAsync(rally.Id, TipoCierreDirecto.Ace, null, detalle);

            await _svc.DeshacerUltimaAccionAsync(rally.Id);

            var acciones = await _db.Acciones.Where(a => a.RallyId == rally.Id).ToListAsync();
            var rallyActualizado = await _db.Rallies.FindAsync(rally.Id);

            Assert.Empty(acciones);
            Assert.Null(rallyActualizado!.DuplaGanadoraId);
            Assert.Null(rallyActualizado.TipoCierre);
            Assert.Equal(0, rallyActualizado.MarcadorDupla1);
            Assert.Equal(0, rallyActualizado.MarcadorDupla2);
        }

        // â”€â”€ Test 2: cierre directo sin acciones â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task CierreDirectoAce_SinAcciones_D1GanaYMarcadorCorrecto()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            // Ace: gana la dupla servante (D1 saca primero)
            await _svc.CierreDirectoAsync(rally.Id, TipoCierreDirecto.Ace);

            var rallyActualizado = await _db.Rallies.FindAsync(rally.Id);

            Assert.Equal(_d1.Id, rallyActualizado!.DuplaGanadoraId);
            Assert.Equal(1, rallyActualizado.MarcadorDupla1);
            Assert.Equal(0, rallyActualizado.MarcadorDupla2);
        }

        // â”€â”€ Test 4: deshacer recepciÃ³n Positivo â†’ saque vuelve a null â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task DeshacerRecepcionPositivo_SaqueVuelveANull()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, _j3.Id, false), null);

            // Pre-assert: saque ya fue derivado a Negativo
            var accionesPre = await _db.Acciones
                .Where(a => a.RallyId == rally.Id).OrderBy(a => a.Secuencia).ToListAsync();
            Assert.Equal(Calidad.Negativo, accionesPre[0].Calidad);

            await _svc.DeshacerUltimaAccionAsync(rally.Id);

            var acciones = await _db.Acciones
                .Where(a => a.RallyId == rally.Id).OrderBy(a => a.Secuencia).ToListAsync();
            Assert.Single(acciones);
            Assert.Null(acciones[0].Calidad);
        }

        // â”€â”€ Test 5: deshacer bloqueo que cerrÃ³ â†’ ataque null y rally reabierto

        [Fact]
        public async Task DeshacerBloqueoQueCerro_AtaqueNullYRallyReabierto()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, _j3.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Ataque, null, _j3.Id, false), null);
            // Bloqueo DobleNegativo â†’ deriva Ataqueâ†’DoblePositivo, cierra D2
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Bloqueo, Calidad.DobleNegativo, _j1.Id, false), null);

            await _svc.DeshacerUltimaAccionAsync(rally.Id);

            var acciones = await _db.Acciones
                .Where(a => a.RallyId == rally.Id).OrderBy(a => a.Secuencia).ToListAsync();
            var rallyActualizado = await _db.Rallies.FindAsync(rally.Id);

            Assert.Equal(3, acciones.Count);
            Assert.Null(acciones[2].Calidad);              // ataque vuelve a null
            Assert.Null(rallyActualizado!.DuplaGanadoraId); // rally reabierto
            Assert.Null(rallyActualizado.TipoCierre);
            Assert.Equal(0, rallyActualizado.MarcadorDupla2); // marcador decrementado
        }

        // â”€â”€ Test 6: deshacer con rally abierto y vacÃ­o â†’ excepciÃ³n â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task DeshacerConRallyVacio_LanzaExcepcion()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _svc.DeshacerUltimaAccionAsync(rally.Id));
        }

        // â”€â”€ Test 7: CierreDirecto(Ace) â†’ Deshacer â†’ rally reabierto 0-0 â”€â”€â”€â”€â”€

        [Fact]
        public async Task CierreDirectoAce_Deshacer_RallyReabiertoMarcadorCero()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);
            await _svc.CierreDirectoAsync(rally.Id, TipoCierreDirecto.Ace);

            // Pre-assert: cerrado
            var pre = await _db.Rallies.FindAsync(rally.Id);
            Assert.NotNull(pre!.DuplaGanadoraId);
            Assert.Equal(1, pre.MarcadorDupla1);

            await _svc.DeshacerUltimaAccionAsync(rally.Id);

            var post = await _db.Rallies.FindAsync(rally.Id);
            Assert.Null(post!.DuplaGanadoraId);
            Assert.Null(post.TipoCierre);
            Assert.Equal(0, post.MarcadorDupla1);
            Assert.Equal(0, post.MarcadorDupla2);
        }

        // â”€â”€ Test 8: CerrarSet dupla ajena â†’ excepciÃ³n; vÃ¡lida â†’ persiste â”€â”€â”€â”€â”€

        [Fact]
        public async Task CerrarSet_DuplaAjenaLanzaExcepcion_DuplaValidaPersiste()
        {
            var (_, set) = await CrearPartidoYSet();

            // Dupla ajena (J1 con J3 = dupla no existente; usamos una dupla nueva)
            var j5 = new Jugador { Nombre = "J5", Apellido = "X", Posicion = PosicionJugador.Bloqueador, RolPrincipal = RolJugador.Rol4 };
            var j6 = new Jugador { Nombre = "J6", Apellido = "X", Posicion = PosicionJugador.Defensor,  RolPrincipal = RolJugador.Rol2 };
            _db.Jugadores.AddRange(j5, j6);
            await _db.SaveChangesAsync();
            var d3 = new Dupla { Jugador1Id = j5.Id, Jugador2Id = j6.Id };
            _db.Duplas.Add(d3);
            await _db.SaveChangesAsync();

            await Assert.ThrowsAsync<ArgumentException>(
                () => _svc.CerrarSetAsync(set.Id, d3.Id));

            await _svc.CerrarSetAsync(set.Id, _d1.Id);
            var setActualizado = await _db.SetsEnVivo.FindAsync(set.Id);
            Assert.Equal(_d1.Id, setActualizado!.DuplaGanadoraId);
        }

        // â”€â”€ Test 9: TipoCierre Ace â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task CierreDirectoAce_TipoCierreEsAce()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);
            await _svc.CierreDirectoAsync(rally.Id, TipoCierreDirecto.Ace);

            var rallyActualizado = await _db.Rallies.FindAsync(rally.Id);
            Assert.Equal(TipoCierreRally.Ace, rallyActualizado!.TipoCierre);
        }

        // â”€â”€ Test 10: TipoCierre PorJuego â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task RegistrarBloqueo_CierrePorJuego_TipoCierreEsPorJuego()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, _j3.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Ataque, null, _j3.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Bloqueo, Calidad.DobleNegativo, _j1.Id, false), null);

            var rallyActualizado = await _db.Rallies.FindAsync(rally.Id);
            Assert.Equal(TipoCierreRally.PorJuego, rallyActualizado!.TipoCierre);
        }

        // â”€â”€ Test 3: dos rallies consecutivos, NumeroRally y marcador acumulado

        [Fact]
        public async Task DosRallies_NumeroRallyYMarcadorAcumuladoCorrectos()
        {
            var (_, set) = await CrearPartidoYSet();

            // Rally 1: D1 gana por Ace
            var rally1 = await _svc.AbrirRallyAsync(set.Id);
            await _svc.CierreDirectoAsync(rally1.Id, TipoCierreDirecto.Ace);

            // Rally 2: abre con el marcador del rally 1 como going-in
            var rally2 = await _svc.AbrirRallyAsync(set.Id);

            Assert.Equal(2, rally2.NumeroRally);
            Assert.Equal(1, rally2.MarcadorDupla1); // going-in: D1 ya lleva 1 punto
            Assert.Equal(0, rally2.MarcadorDupla2);

            // Rally 2: D2 gana por ErrorSaque (sacadora = D1, error â†’ rival = D2)
            await _svc.CierreDirectoAsync(rally2.Id, TipoCierreDirecto.ErrorSaque);

            var rally2Actualizado = await _db.Rallies.FindAsync(rally2.Id);

            Assert.Equal(_d2.Id, rally2Actualizado!.DuplaGanadoraId);
            Assert.Equal(1, rally2Actualizado.MarcadorDupla1);
            Assert.Equal(1, rally2Actualizado.MarcadorDupla2);
        }

        // â”€â”€ Test 11: Armado Negativo no cierra ni deriva â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task ArmadoNegativo_NoDeriva_NoSeCierra()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, _j3.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Armado, Calidad.Negativo, _j4.Id, false), null);

            var rallyDb = await _db.Rallies.FindAsync(rally.Id);
            Assert.Null(rallyDb!.DuplaGanadoraId);

            // Ninguna acciÃ³n con fundamento Armado tiene calidad derivada (no hay combinada)
            var acciones = await _db.Acciones
                .Where(a => a.RallyId == rally.Id)
                .ToListAsync();
            var saque = acciones.First(a => a.Fundamento == Fundamento.Saque);
            // Saque fue derivado por la Recepcion+, no por el Armado
            Assert.Equal(Calidad.Negativo, saque.Calidad);
        }

        // â”€â”€ Test 12: DegradePrimerContacto â€” Recepcion+ â†’ degrada y agrega FreeBall

        [Fact]
        public async Task DegradePrimerContacto_RecepcionPositiva_DegradaYAgregaFreeBall()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, _j3.Id, false), null);

            await _svc.DegradePrimerContactoAsync(rally.Id);

            var acciones = await _db.Acciones
                .Where(a => a.RallyId == rally.Id)
                .OrderBy(a => a.Secuencia)
                .ToListAsync();

            // Recepcion degradada a Negativo
            var recep = acciones.First(a => a.Fundamento == Fundamento.Recepcion);
            Assert.Equal(Calidad.Negativo, recep.Calidad);

            // Saque recalculado a Positivo (combinada: Recepcionâˆ’ â†’ Saque+)
            var saque = acciones.First(a => a.Fundamento == Fundamento.Saque);
            Assert.Equal(Calidad.Positivo, saque.Calidad);

            // FreeBall insertado (compaÃ±ero del receptor = _j4), sin calidad estadÃ­stica
            var fb = acciones.First(a => a.Fundamento == Fundamento.FreeBall);
            Assert.Null(fb.Calidad);
            Assert.Equal(_j4.Id, fb.JugadorId);

            // Rally sigue abierto
            var rallyDb = await _db.Rallies.FindAsync(rally.Id);
            Assert.Null(rallyDb!.DuplaGanadoraId);
        }

        // â”€â”€ Test 13: DegradePrimerContacto â€” Recepcion ya Negativa â†’ no-op calidad, igual agrega FreeBall

        [Fact]
        public async Task DegradePrimerContacto_RecepcionYaNegativa_SoloAgregaFreeBall()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Negativo, _j3.Id, false), null);

            // La recepcion ya es Negativo â†’ no-op de degradaciÃ³n, pero sigue el flujo
            await _svc.DegradePrimerContactoAsync(rally.Id);

            var acciones = await _db.Acciones
                .Where(a => a.RallyId == rally.Id)
                .OrderBy(a => a.Secuencia)
                .ToListAsync();

            var recep = acciones.First(a => a.Fundamento == Fundamento.Recepcion);
            Assert.Equal(Calidad.Negativo, recep.Calidad);

            // FreeBall igual se agrega (sin calidad estadÃ­stica)
            Assert.Contains(acciones, a => a.Fundamento == Fundamento.FreeBall && a.Calidad == null);
        }

        // â”€â”€ Test 14: DegradePrimerContacto â€” Defensa degrada sin Armado â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task DegradePrimerContacto_DefensaExclamativa_DegradaSinArmado()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            // Saque â†’ Recep â†’ Ataque â†’ Defensa!
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, _j3.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Ataque, null, _j3.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Defensa, Calidad.Exclamativa, _j1.Id, false), null);

            await _svc.DegradePrimerContactoAsync(rally.Id);

            var acciones = await _db.Acciones
                .Where(a => a.RallyId == rally.Id)
                .ToListAsync();

            // Defensa degradada a Negativo
            var defensa = acciones.First(a => a.Fundamento == Fundamento.Defensa);
            Assert.Equal(Calidad.Negativo, defensa.Calidad);

            // Sin acciÃ³n Armado ni FreeBall (Defensa ya cruza posesiÃ³n por Regla 11)
            Assert.DoesNotContain(acciones, a => a.Fundamento == Fundamento.Armado);
            Assert.DoesNotContain(acciones, a => a.Fundamento == Fundamento.FreeBall);

            // Rally sigue abierto
            var rallyDb = await _db.Rallies.FindAsync(rally.Id);
            Assert.Null(rallyDb!.DuplaGanadoraId);
        }

        // â”€â”€ Test 15b: DegradePrimerContacto â†’ Deshacer elimina FreeBall â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task DegradePrimerContacto_Deshacer_EliminaFreeBallRallyAbierto()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, _j3.Id, false), null);

            await _svc.DegradePrimerContactoAsync(rally.Id);
            await _svc.DeshacerUltimaAccionAsync(rally.Id);

            var acciones = await _db.Acciones
                .Where(a => a.RallyId == rally.Id)
                .ToListAsync();

            // FreeBall eliminado
            Assert.DoesNotContain(acciones, a => a.Fundamento == Fundamento.FreeBall);

            // Rally sigue abierto
            var rallyDb = await _db.Rallies.FindAsync(rally.Id);
            Assert.Null(rallyDb!.DuplaGanadoraId);

            // LimitaciÃ³n conocida: recepcion queda en Negativo (calidad previa no se restaura)
            var recep = acciones.First(a => a.Fundamento == Fundamento.Recepcion);
            Assert.Equal(Calidad.Negativo, recep.Calidad);
        }

        // â”€â”€ Test 16: Bloqueo con Exclamativa â†’ ArgumentException, no persiste â”€

        [Fact]
        public async Task RegistrarBloqueoExclamativa_LanzaArgumentException_AccionNoGuardada()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            // Secuencia previa: Saque â†’ Recepcion â†’ Ataque (sin calidad)
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, _j3.Id, false), null);
            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Ataque, null, _j3.Id, false), null);

            int accionesAntes = await _db.Acciones.CountAsync(a => a.RallyId == rally.Id);

            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _svc.RegistrarAccionAsync(rally.Id,
                    new CargaAccion(Fundamento.Bloqueo, Calidad.Exclamativa, _j1.Id, false), null));

            Assert.Contains("no es una combinaciÃ³n vÃ¡lida", ex.Message);
            int accionesDespues = await _db.Acciones.CountAsync(a => a.RallyId == rally.Id);
            Assert.Equal(accionesAntes, accionesDespues);
        }

        // â”€â”€ Test 15: DegradePrimerContacto sin Recepcion/Defensa â†’ excepciÃ³n â”€â”€

        [Fact]
        public async Task DegradePrimerContacto_SoloSaque_LanzaExcepcion()
        {
            var (_, set) = await CrearPartidoYSet();
            var rally = await _svc.AbrirRallyAsync(set.Id);

            await _svc.RegistrarAccionAsync(rally.Id,
                new CargaAccion(Fundamento.Saque, null, _j1.Id, false), null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _svc.DegradePrimerContactoAsync(rally.Id));
        }
    }
}
```

## SandStats.Tests\CierresTests.cs
```csharp
using SandStats.Models;
using SandStats.Models.EnVivo;
using SandStats.Services.EnVivo;
using Xunit;

namespace SandStats.Tests
{
    public class CierresTests
    {
        private const int D1 = 1, D2 = 2;
        private const int A = 10, B = 11, C = 20, D = 21;

        private static ContextoRally Ctx(int[] ganadores = null!) => new()
        {
            Dupla1Id = D1,
            Dupla2Id = D2,
            JugadoresPorDupla = new Dictionary<int, IReadOnlyList<JugadorEnCancha>>
            {
                [D1] = [new JugadorEnCancha(A, PosicionJugador.Bloqueador), new JugadorEnCancha(B, PosicionJugador.Defensor)],
                [D2] = [new JugadorEnCancha(C, PosicionJugador.Bloqueador), new JugadorEnCancha(D, PosicionJugador.Defensor)]
            },
            SacadorInicialDupla1JugadorId = A,
            SacadorInicialDupla2JugadorId = C,
            DuplaQueSacaPrimeroId = D1,
            GanadoresRalliesPrevios = ganadores ?? Array.Empty<int>(),
            AccionesRallyActual = Array.Empty<Accion>(),
            Combinadas = TodasLasCombinadas()
        };

        private static IReadOnlyList<ModificadorCombinada> TodasLasCombinadas() =>
        [
            new ModificadorCombinada { Id = 1,  FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.DoblePositivo, FundamentoDerivado = Fundamento.Saque, CalidadDerivada = Calidad.Negativo },
            new ModificadorCombinada { Id = 2,  FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.Positivo,      FundamentoDerivado = Fundamento.Saque, CalidadDerivada = Calidad.Negativo },
            new ModificadorCombinada { Id = 3,  FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.Exclamativa,   FundamentoDerivado = Fundamento.Saque, CalidadDerivada = Calidad.Exclamativa },
            new ModificadorCombinada { Id = 4,  FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.Slash,         FundamentoDerivado = Fundamento.Saque, CalidadDerivada = Calidad.Slash },
            new ModificadorCombinada { Id = 5,  FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.Negativo,      FundamentoDerivado = Fundamento.Saque, CalidadDerivada = Calidad.Positivo },
            new ModificadorCombinada { Id = 6,  FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.DobleNegativo, FundamentoDerivado = Fundamento.Saque, CalidadDerivada = Calidad.DoblePositivo },
            new ModificadorCombinada { Id = 7,  FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.DoblePositivo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Slash },
            new ModificadorCombinada { Id = 8,  FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.Positivo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Negativo },
            new ModificadorCombinada { Id = 9,  FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.Slash,         FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo },
            new ModificadorCombinada { Id = 10, FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.Negativo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo },
            new ModificadorCombinada { Id = 11, FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.DobleNegativo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.DoblePositivo },
            new ModificadorCombinada { Id = 12, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.DoblePositivo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Negativo },
            new ModificadorCombinada { Id = 13, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.Positivo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Negativo },
            new ModificadorCombinada { Id = 14, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.Exclamativa,   FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo },
            new ModificadorCombinada { Id = 15, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.Slash,         FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo },
            new ModificadorCombinada { Id = 16, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.Negativo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo },
            new ModificadorCombinada { Id = 17, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.DobleNegativo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.DoblePositivo }
        ];

        // â”€â”€ Cierres por carga â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Recepcion_DobleNegativo_GanaSacadora()
        {
            // D1 saca (sin rallies previos), receptor es C (D2)
            var r = new MotorRally().Registrar(Ctx(), new CargaAccion(Fundamento.Recepcion, Calidad.DobleNegativo, C, false));
            Assert.Equal(D1, r.Cierre!.DuplaGanadoraId);
        }

        [Fact]
        public void Ataque_DoblePositivo_GanaAtacante()
        {
            var r = new MotorRally().Registrar(Ctx(), new CargaAccion(Fundamento.Ataque, Calidad.DoblePositivo, A, false));
            Assert.Equal(D1, r.Cierre!.DuplaGanadoraId);
        }

        [Fact]
        public void Ataque_DobleNegativo_GanaRivalAtacante()
        {
            var r = new MotorRally().Registrar(Ctx(), new CargaAccion(Fundamento.Ataque, Calidad.DobleNegativo, A, false));
            Assert.Equal(D2, r.Cierre!.DuplaGanadoraId);
        }

        [Fact]
        public void Bloqueo_DoblePositivo_GanaBloqueador()
        {
            var r = new MotorRally().Registrar(Ctx(), new CargaAccion(Fundamento.Bloqueo, Calidad.DoblePositivo, C, false));
            Assert.Equal(D2, r.Cierre!.DuplaGanadoraId);
        }

        [Fact]
        public void Bloqueo_DobleNegativo_GanaRivalBloqueador()
        {
            var r = new MotorRally().Registrar(Ctx(), new CargaAccion(Fundamento.Bloqueo, Calidad.DobleNegativo, C, false));
            Assert.Equal(D1, r.Cierre!.DuplaGanadoraId);
        }

        [Fact]
        public void Defensa_DobleNegativo_GanaRivalDefensor()
        {
            var r = new MotorRally().Registrar(Ctx(), new CargaAccion(Fundamento.Defensa, Calidad.DobleNegativo, A, false));
            Assert.Equal(D2, r.Cierre!.DuplaGanadoraId);
        }

        // â”€â”€ Cierres directos â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void CierreDirecto_Ace_GanaSacadora()
        {
            var r = new MotorRally().CierreDirecto(Ctx(), TipoCierreDirecto.Ace);
            Assert.Equal(D1, r.Cierre!.DuplaGanadoraId);
        }

        [Fact]
        public void CierreDirecto_ErrorSaque_GanaReceptora()
        {
            var r = new MotorRally().CierreDirecto(Ctx(), TipoCierreDirecto.ErrorSaque);
            Assert.Equal(D2, r.Cierre!.DuplaGanadoraId);
        }

        [Fact]
        public void CierreDirecto_ErrorVario_GanaDuplaParam()
        {
            var r = new MotorRally().CierreDirecto(Ctx(), TipoCierreDirecto.ErrorVario, D2);
            Assert.Equal(D2, r.Cierre!.DuplaGanadoraId);
        }

        [Fact]
        public void CierreDirecto_CierreRapido_GanaDuplaParam()
        {
            var r = new MotorRally().CierreDirecto(Ctx(), TipoCierreDirecto.CierreRapido, D1);
            Assert.Equal(D1, r.Cierre!.DuplaGanadoraId);
        }

        // â”€â”€ Continuaciones (Cierre == null) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Recepcion_Positivo_Continua()
        {
            var r = new MotorRally().Registrar(Ctx(), new CargaAccion(Fundamento.Recepcion, Calidad.Positivo, C, false));
            Assert.Null(r.Cierre);
        }

        [Fact]
        public void Bloqueo_Slash_Continua()
        {
            var r = new MotorRally().Registrar(Ctx(), new CargaAccion(Fundamento.Bloqueo, Calidad.Slash, C, false));
            Assert.Null(r.Cierre);
        }

        [Fact]
        public void Defensa_Negativo_Continua()
        {
            var r = new MotorRally().Registrar(Ctx(), new CargaAccion(Fundamento.Defensa, Calidad.Negativo, A, false));
            Assert.Null(r.Cierre);
        }

        // â”€â”€ Ataque pendiente de derivaciÃ³n â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Ataque_CalidadNull_NiDerivaÐiCierra()
        {
            var r = new MotorRally().Registrar(Ctx(), new CargaAccion(Fundamento.Ataque, null, A, false));
            Assert.Null(r.Derivacion);
            Assert.Null(r.Cierre);
        }
    }
}
```

## SandStats.Tests\CombinadasTests.cs
```csharp
using SandStats.Models;
using SandStats.Models.EnVivo;
using SandStats.Services.EnVivo;
using Xunit;

namespace SandStats.Tests
{
    public class CombinadasTests
    {
        private const int D1 = 1, D2 = 2, A = 10;

        private static readonly IReadOnlyList<ModificadorCombinada> TodasLasCombinadas =
        [
            new() { Id =  1, FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.DoblePositivo, FundamentoDerivado = Fundamento.Saque,  CalidadDerivada = Calidad.Negativo      },
            new() { Id =  2, FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.Positivo,      FundamentoDerivado = Fundamento.Saque,  CalidadDerivada = Calidad.Negativo      },
            new() { Id =  3, FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.Exclamativa,   FundamentoDerivado = Fundamento.Saque,  CalidadDerivada = Calidad.Exclamativa   },
            new() { Id =  4, FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.Slash,         FundamentoDerivado = Fundamento.Saque,  CalidadDerivada = Calidad.Slash         },
            new() { Id =  5, FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.Negativo,      FundamentoDerivado = Fundamento.Saque,  CalidadDerivada = Calidad.Positivo      },
            new() { Id =  6, FundamentoCargado = Fundamento.Recepcion, CalidadCargada = Calidad.DobleNegativo, FundamentoDerivado = Fundamento.Saque,  CalidadDerivada = Calidad.DoblePositivo },
            new() { Id =  7, FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.DoblePositivo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Slash         },
            new() { Id =  8, FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.Positivo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Negativo      },
            new() { Id =  9, FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.Slash,         FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo      },
            new() { Id = 10, FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.Negativo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo      },
            new() { Id = 11, FundamentoCargado = Fundamento.Bloqueo,   CalidadCargada = Calidad.DobleNegativo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.DoblePositivo },
            new() { Id = 12, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.DoblePositivo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Negativo      },
            new() { Id = 13, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.Positivo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Negativo      },
            new() { Id = 14, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.Exclamativa,   FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo      },
            new() { Id = 15, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.Slash,         FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo      },
            new() { Id = 16, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.Negativo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo      },
            new() { Id = 17, FundamentoCargado = Fundamento.Defensa,   CalidadCargada = Calidad.DobleNegativo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.DoblePositivo },
        ];

        public static IEnumerable<object[]> FilasSeed =>
        [
            [Fundamento.Recepcion, Calidad.DoblePositivo, Fundamento.Saque,  Calidad.Negativo     ],
            [Fundamento.Recepcion, Calidad.Positivo,      Fundamento.Saque,  Calidad.Negativo     ],
            [Fundamento.Recepcion, Calidad.Exclamativa,   Fundamento.Saque,  Calidad.Exclamativa  ],
            [Fundamento.Recepcion, Calidad.Slash,         Fundamento.Saque,  Calidad.Slash        ],
            [Fundamento.Recepcion, Calidad.Negativo,      Fundamento.Saque,  Calidad.Positivo     ],
            [Fundamento.Recepcion, Calidad.DobleNegativo, Fundamento.Saque,  Calidad.DoblePositivo],
            [Fundamento.Bloqueo,   Calidad.DoblePositivo, Fundamento.Ataque, Calidad.Slash        ],
            [Fundamento.Bloqueo,   Calidad.Positivo,      Fundamento.Ataque, Calidad.Negativo     ],
            [Fundamento.Bloqueo,   Calidad.Slash,         Fundamento.Ataque, Calidad.Positivo     ],
            [Fundamento.Bloqueo,   Calidad.Negativo,      Fundamento.Ataque, Calidad.Positivo     ],
            [Fundamento.Bloqueo,   Calidad.DobleNegativo, Fundamento.Ataque, Calidad.DoblePositivo],
            [Fundamento.Defensa,   Calidad.DoblePositivo, Fundamento.Ataque, Calidad.Negativo     ],
            [Fundamento.Defensa,   Calidad.Positivo,      Fundamento.Ataque, Calidad.Negativo     ],
            [Fundamento.Defensa,   Calidad.Exclamativa,   Fundamento.Ataque, Calidad.Positivo     ],
            [Fundamento.Defensa,   Calidad.Slash,         Fundamento.Ataque, Calidad.Positivo     ],
            [Fundamento.Defensa,   Calidad.Negativo,      Fundamento.Ataque, Calidad.Positivo     ],
            [Fundamento.Defensa,   Calidad.DobleNegativo, Fundamento.Ataque, Calidad.DoblePositivo],
        ];

        private static ContextoRally CtxConCombinadas() => new()
        {
            Dupla1Id = D1,
            Dupla2Id = D2,
            JugadoresPorDupla = new Dictionary<int, IReadOnlyList<JugadorEnCancha>>
            {
                [D1] = [new JugadorEnCancha(A, PosicionJugador.Bloqueador), new JugadorEnCancha(11, PosicionJugador.Defensor)],
                [D2] = [new JugadorEnCancha(20, PosicionJugador.Bloqueador), new JugadorEnCancha(21, PosicionJugador.Defensor)]
            },
            SacadorInicialDupla1JugadorId = A,
            SacadorInicialDupla2JugadorId = 20,
            DuplaQueSacaPrimeroId = D1,
            GanadoresRalliesPrevios = Array.Empty<int>(),
            AccionesRallyActual = Array.Empty<Accion>(),
            Combinadas = TodasLasCombinadas
        };

        [Theory]
        [MemberData(nameof(FilasSeed))]
        public void Combinada_DerivÐ°LaCalidadCorrecta(
            Fundamento fc, Calidad cc, Fundamento fd, Calidad cd)
        {
            var resultado = new MotorRally().Registrar(CtxConCombinadas(), new CargaAccion(fc, cc, A, false));
            Assert.Equal(new Derivacion(fd, cd), resultado.Derivacion);
        }
    }
}
```

## SandStats.Tests\EnVivoEndpointsTests.cs
```csharp
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using SandStats.Data;
using SandStats.Models;
using SandStats.Models.EnVivo;
using Xunit;

namespace SandStats.Tests
{
    public class EnVivoEndpointsTests : IClassFixture<EnVivoWebFactory>
    {
        private readonly HttpClient _client;
        private readonly EnVivoWebFactory _factory;

        public EnVivoEndpointsTests(EnVivoWebFactory factory)
        {
            _factory = factory;
            _client  = factory.CreateClient();
        }

        // Seed: 4 jugadores + 2 duplas nuevos por test (IDs auto-incrementados, sin interferencia)
        private async Task<(int d1Id, int d2Id, int j1Id, int j3Id)> SeedDuplasAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var j1 = new Jugador { Nombre = "A", Apellido = "D1", Posicion = PosicionJugador.Bloqueador, RolPrincipal = RolJugador.Rol4 };
            var j2 = new Jugador { Nombre = "B", Apellido = "D1", Posicion = PosicionJugador.Defensor,   RolPrincipal = RolJugador.Rol2 };
            var j3 = new Jugador { Nombre = "C", Apellido = "D2", Posicion = PosicionJugador.Bloqueador, RolPrincipal = RolJugador.Rol4 };
            var j4 = new Jugador { Nombre = "D", Apellido = "D2", Posicion = PosicionJugador.Defensor,   RolPrincipal = RolJugador.Rol2 };
            db.Jugadores.AddRange(j1, j2, j3, j4);
            await db.SaveChangesAsync();

            var d1 = new Dupla { Jugador1Id = j1.Id, Jugador2Id = j2.Id };
            var d2 = new Dupla { Jugador1Id = j3.Id, Jugador2Id = j4.Id };
            db.Duplas.AddRange(d1, d2);
            await db.SaveChangesAsync();

            return (d1.Id, d2.Id, j1.Id, j3.Id);
        }

        private async Task<(int partidoId, int setId, int rallyId)> CrearPartidoSetRallyAsync(
            int d1Id, int d2Id, int j1Id, int j3Id)
        {
            var rP = await _client.PostAsJsonAsync("/api/envivo/partidos",
                new { dupla1Id = d1Id, dupla2Id = d2Id, torneo = "T", fecha = DateTime.UtcNow });
            rP.EnsureSuccessStatusCode();
            var pj = JsonDocument.Parse(await rP.Content.ReadAsStringAsync()).RootElement;
            int pId = pj.GetProperty("id").GetInt32();

            var rS = await _client.PostAsJsonAsync($"/api/envivo/partidos/{pId}/sets",
                new { numeroSet = 1, sacadorInicialD1Id = j1Id, sacadorInicialD2Id = j3Id, duplaQueSacaPrimeroId = d1Id });
            rS.EnsureSuccessStatusCode();
            var sj = JsonDocument.Parse(await rS.Content.ReadAsStringAsync()).RootElement;
            int sId = sj.GetProperty("id").GetInt32();

            var rR = await _client.PostAsJsonAsync($"/api/envivo/sets/{sId}/rallies", new { });
            rR.EnsureSuccessStatusCode();
            var rj = JsonDocument.Parse(await rR.Content.ReadAsStringAsync()).RootElement;
            int rallyId = rj.GetProperty("id").GetInt32();

            return (pId, sId, rallyId);
        }

        // â”€â”€ Test 1: flujo completo 4 acciones â†’ cerrado, marcador 0-1 â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task FlujoCopleto_4Acciones_EstadoFinalCerradoYMarcadorCorrecto()
        {
            var (d1Id, d2Id, j1Id, j3Id) = await SeedDuplasAsync();
            var (_, _, rallyId) = await CrearPartidoSetRallyAsync(d1Id, d2Id, j1Id, j3Id);

            // Saque(null, J1)
            await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/acciones",
                new { fundamento = "Saque", calidad = (string?)null, jugadorId = j1Id, esDe2da = false, detalle = (object?)null });

            // Recepcion(Positivo, J3) â†’ deriva Saqueâ†’Negativo
            await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/acciones",
                new { fundamento = "Recepcion", calidad = "Positivo", jugadorId = j3Id, esDe2da = false, detalle = (object?)null });

            // Ataque(null, J3)
            await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/acciones",
                new { fundamento = "Ataque", calidad = (string?)null, jugadorId = j3Id, esDe2da = false, detalle = (object?)null });

            // Bloqueo(DobleNegativo, J1) â†’ deriva Ataqueâ†’DoblePositivo, cierra D2
            var r4 = await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/acciones",
                new { fundamento = "Bloqueo", calidad = "DobleNegativo", jugadorId = j1Id, esDe2da = false, detalle = (object?)null });
            r4.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await r4.Content.ReadAsStringAsync());
            var root = doc.RootElement;

            Assert.True(root.GetProperty("cerrado").GetBoolean());
            Assert.Equal(0, root.GetProperty("marcadorDupla1").GetInt32());
            Assert.Equal(1, root.GetProperty("marcadorDupla2").GetInt32());
        }

        // â”€â”€ Test 2: registrar acciÃ³n en rally cerrado â†’ 400 â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task RegistrarAccion_RallyCerrado_Retorna400()
        {
            var (d1Id, d2Id, j1Id, j3Id) = await SeedDuplasAsync();
            var (_, _, rallyId) = await CrearPartidoSetRallyAsync(d1Id, d2Id, j1Id, j3Id);

            // Cerrar el rally
            var rc = await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/cierre-directo",
                new { tipo = "Ace", duplaGanadoraId = (int?)null });
            rc.EnsureSuccessStatusCode();

            // Intentar registrar acciÃ³n en rally ya cerrado
            var r = await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/acciones",
                new { fundamento = "Saque", calidad = (string?)null, jugadorId = j1Id, esDe2da = false, detalle = (object?)null });

            Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
        }

        // â”€â”€ Test 3: deshacer en rally abierto sin acciones â†’ 400 â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task Deshacer_RallyAbiertoVacio_Retorna400()
        {
            var (d1Id, d2Id, j1Id, j3Id) = await SeedDuplasAsync();
            var (_, _, rallyId) = await CrearPartidoSetRallyAsync(d1Id, d2Id, j1Id, j3Id);

            var r = await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/deshacer", new { });
            Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
        }

        // â”€â”€ Test 4: rally inexistente â†’ 404 â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task RallyInexistente_Retorna404()
        {
            var r = await _client.PostAsJsonAsync("/api/envivo/rallies/99999/acciones",
                new { fundamento = "Saque", calidad = (string?)null, jugadorId = 1, esDe2da = false, detalle = (object?)null });
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
        }

        // â”€â”€ Test 5: GET estado con set sin rallies â†’ 200, cerrado=true â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task GetEstado_SetSinRallies_Retorna200CerradoTrue()
        {
            var (d1Id, d2Id, j1Id, j3Id) = await SeedDuplasAsync();

            var rP = await _client.PostAsJsonAsync("/api/envivo/partidos",
                new { dupla1Id = d1Id, dupla2Id = d2Id, torneo = "T", fecha = DateTime.UtcNow });
            rP.EnsureSuccessStatusCode();
            int pId = JsonDocument.Parse(await rP.Content.ReadAsStringAsync())
                .RootElement.GetProperty("id").GetInt32();

            var rS = await _client.PostAsJsonAsync($"/api/envivo/partidos/{pId}/sets",
                new { numeroSet = 1, sacadorInicialD1Id = j1Id, sacadorInicialD2Id = j3Id, duplaQueSacaPrimeroId = d1Id });
            rS.EnsureSuccessStatusCode();
            int setId = JsonDocument.Parse(await rS.Content.ReadAsStringAsync())
                .RootElement.GetProperty("id").GetInt32();

            var r = await _client.GetAsync($"/api/envivo/sets/{setId}/estado");

            Assert.Equal(HttpStatusCode.OK, r.StatusCode);

            using var doc = JsonDocument.Parse(await r.Content.ReadAsStringAsync());
            var root = doc.RootElement;
            Assert.True(root.GetProperty("cerrado").GetBoolean());
            Assert.Equal(0, root.GetProperty("marcadorDupla1").GetInt32());
            Assert.Equal(0, root.GetProperty("marcadorDupla2").GetInt32());
        }

        // â”€â”€ Test 6: GET estado con rally vacÃ­o â†’ sugerencia es Saque â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public async Task GetEstado_RallyVacio_SugerenciaEsSaque()
        {
            var (d1Id, d2Id, j1Id, j3Id) = await SeedDuplasAsync();
            var (_, setId, _) = await CrearPartidoSetRallyAsync(d1Id, d2Id, j1Id, j3Id);

            var r = await _client.GetAsync($"/api/envivo/sets/{setId}/estado");
            r.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await r.Content.ReadAsStringAsync());
            var opciones = doc.RootElement
                .GetProperty("sugerencia")
                .GetProperty("opciones");

            Assert.Equal("Saque", opciones[0].GetProperty("fundamento").GetString());
        }
    }
}
```

## SandStats.Tests\EnVivoWebFactory.cs
```csharp
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SandStats.Data;

namespace SandStats.Tests
{
    public class EnVivoWebFactory : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _conn;

        public EnVivoWebFactory()
        {
            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureServices(services =>
            {
                // Reemplazar DbContext por SQLite :memory: compartida entre requests y seeds
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(opts => opts.UseSqlite(_conn));

                // Auth bypass: el TestAuthHandler siempre autentica sin cookies
                services.AddAuthentication()
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

                services.PostConfigureAll<AuthenticationOptions>(opts =>
                {
                    opts.DefaultScheme               = "Test";
                    opts.DefaultAuthenticateScheme   = "Test";
                    opts.DefaultChallengeScheme      = "Test";
                    opts.DefaultForbidScheme         = "Test";
                });
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);
            using var scope = host.Services.CreateScope();
            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>()
                .Database.EnsureCreated();
            return host;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) _conn.Close();
        }
    }
}
```

## SandStats.Tests\InferenciaAtaqueTests.cs
```csharp
using SandStats.Models;
using SandStats.Models.EnVivo;
using SandStats.Services.EnVivo;
using Xunit;

namespace SandStats.Tests
{
    public class InferenciaAtaqueTests
    {
        // â”€â”€ Spike siempre devuelve Atq{n} â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Spike_DevuelveAtqN_IndependienteDeLadoYRol()
        {
            Assert.Equal(TipoAcciones.Atq4,
                InferenciaAtaque.Inferir("Spike", TipoLado.Bueno, ZonaCancha.Zona4, RolJugador.Rol4));
        }

        // â”€â”€ Tip Rol4 Bueno: lÃ­nea = 1,9,2 â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Theory]
        [InlineData(ZonaCancha.Zona1, TipoAcciones.Tl1)]
        [InlineData(ZonaCancha.Zona9, TipoAcciones.Tl9)]
        [InlineData(ZonaCancha.Zona2, TipoAcciones.Tl2)]
        public void Tip_Rol4_Bueno_ZonaLinea_DevuelveTl(ZonaCancha zona, TipoAcciones esperado)
        {
            Assert.Equal(esperado,
                InferenciaAtaque.Inferir("Tip", TipoLado.Bueno, zona, RolJugador.Rol4));
        }

        [Theory]
        [InlineData(ZonaCancha.Zona3, TipoAcciones.Td3)]
        [InlineData(ZonaCancha.Zona7, TipoAcciones.Td7)]
        public void Tip_Rol4_Bueno_ZonaDiagonal_DevuelveTd(ZonaCancha zona, TipoAcciones esperado)
        {
            Assert.Equal(esperado,
                InferenciaAtaque.Inferir("Tip", TipoLado.Bueno, zona, RolJugador.Rol4));
        }

        // â”€â”€ Tip Rol4 AtrÃ¡s: lÃ­nea = 5,7,4 â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Theory]
        [InlineData(ZonaCancha.Zona5, TipoAcciones.Tl5)]
        [InlineData(ZonaCancha.Zona4, TipoAcciones.Tl4)]
        public void Tip_Rol4_Atras_ZonaLinea_DevuelveTl(ZonaCancha zona, TipoAcciones esperado)
        {
            Assert.Equal(esperado,
                InferenciaAtaque.Inferir("Tip", TipoLado.Atras, zona, RolJugador.Rol4));
        }

        [Theory]
        [InlineData(ZonaCancha.Zona1, TipoAcciones.Td1)]
        [InlineData(ZonaCancha.Zona9, TipoAcciones.Td9)]
        public void Tip_Rol4_Atras_ZonaDiagonal_DevuelveTd(ZonaCancha zona, TipoAcciones esperado)
        {
            Assert.Equal(esperado,
                InferenciaAtaque.Inferir("Tip", TipoLado.Atras, zona, RolJugador.Rol4));
        }

        // â”€â”€ Tip Rol2 Bueno (= Rol4 AtrÃ¡s): lÃ­nea = 5,7,4 â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Tip_Rol2_Bueno_Zona5_EsLinea()
        {
            Assert.Equal(TipoAcciones.Tl5,
                InferenciaAtaque.Inferir("Tip", TipoLado.Bueno, ZonaCancha.Zona5, RolJugador.Rol2));
        }

        [Fact]
        public void Tip_Rol2_Bueno_Zona3_EsDiagonal()
        {
            Assert.Equal(TipoAcciones.Td3,
                InferenciaAtaque.Inferir("Tip", TipoLado.Bueno, ZonaCancha.Zona3, RolJugador.Rol2));
        }

        // â”€â”€ Tip Rol2 AtrÃ¡s (= Rol4 Bueno): lÃ­nea = 1,9,2 â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Tip_Rol2_Atras_Zona1_EsLinea()
        {
            Assert.Equal(TipoAcciones.Tl1,
                InferenciaAtaque.Inferir("Tip", TipoLado.Atras, ZonaCancha.Zona1, RolJugador.Rol2));
        }
    }
}
```

## SandStats.Tests\MarcadorTests.cs
```csharp
using SandStats.Services.EnVivo;
using Xunit;

namespace SandStats.Tests
{
    public class MarcadorTests
    {
        private const int D1 = 1, D2 = 2;
        private readonly MotorRally _motor = new();

        private ResultadoMarcador Eval(int p1, int p2, int set = 1, int s1 = 0, int s2 = 0)
            => _motor.EvaluarMarcador(D1, D2, p1, p2, set, s1, s2);

        // â”€â”€ Set termina â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Set1_20_20_NoCierra()
        {
            var r = Eval(20, 20);
            Assert.False(r.SetTerminado);
        }

        [Fact]
        public void Set1_21_20_DiferenciaUno_NoCierra()
        {
            var r = Eval(21, 20);
            Assert.False(r.SetTerminado);
        }

        [Fact]
        public void Set1_21_19_CierraDupla1()
        {
            var r = Eval(21, 19);
            Assert.True(r.SetTerminado);
            Assert.Equal(D1, r.DuplaGanadoraSetId);
        }

        [Fact]
        public void Set1_22_20_CierraDupla1()
        {
            var r = Eval(22, 20);
            Assert.True(r.SetTerminado);
            Assert.Equal(D1, r.DuplaGanadoraSetId);
        }

        [Fact]
        public void Set3_14_14_NoCierra()
        {
            var r = Eval(14, 14, set: 3, s1: 1, s2: 1);
            Assert.False(r.SetTerminado);
        }

        [Fact]
        public void Set3_15_13_CierraDupla1()
        {
            var r = Eval(15, 13, set: 3, s1: 1, s2: 1);
            Assert.True(r.SetTerminado);
            Assert.Equal(D1, r.DuplaGanadoraSetId);
        }

        // â”€â”€ Partido termina â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Partido_21_19_ConSetsGanados1_0_PartidoTerminado()
        {
            var r = Eval(21, 19, set: 2, s1: 1, s2: 0);
            Assert.True(r.SetTerminado);
            Assert.True(r.PartidoTerminado);
            Assert.Equal(D1, r.DuplaGanadoraSetId);
        }

        [Fact]
        public void Partido_21_19_SinSetsGanadosPrevios_PartidoNoTerminado()
        {
            var r = Eval(21, 19, set: 1, s1: 0, s2: 0);
            Assert.True(r.SetTerminado);
            Assert.False(r.PartidoTerminado);
        }

        // â”€â”€ Cambio de lado â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Set1_4_3_Total7_CambioDeLado()
        {
            var r = Eval(4, 3);
            Assert.True(r.CambioDeLado);
        }

        [Fact]
        public void Set1_8_6_Total14_CambioDeLado()
        {
            var r = Eval(8, 6);
            Assert.True(r.CambioDeLado);
        }

        [Fact]
        public void Set3_3_2_Total5_CambioDeLado()
        {
            var r = Eval(3, 2, set: 3);
            Assert.True(r.CambioDeLado);
        }

        [Fact]
        public void Set1_0_0_NoCambioDeLado()
        {
            var r = Eval(0, 0);
            Assert.False(r.CambioDeLado);
        }

        [Fact]
        public void Set1_21_14_SetTerminado_NoCambioDeLadoAunqueTotal35()
        {
            // total=35=7x5, pero el set cerrÃ³ en ese punto â†’ CambioDeLado=false
            var r = Eval(21, 14);
            Assert.True(r.SetTerminado);
            Assert.False(r.CambioDeLado);
        }
    }
}
```

## SandStats.Tests\RotacionSaqueTests.cs
```csharp
using SandStats.Models;
using SandStats.Models.EnVivo;
using SandStats.Services.EnVivo;
using Xunit;

namespace SandStats.Tests
{
    public class RotacionSaqueTests
    {
        private const int D1 = 1, D2 = 2;
        private const int A = 10, B = 11, C = 20, D = 21;

        private static ContextoRally Ctx(int duplaQueSacaPrimero, int[] ganadores) => new()
        {
            Dupla1Id = D1,
            Dupla2Id = D2,
            JugadoresPorDupla = new Dictionary<int, IReadOnlyList<JugadorEnCancha>>
            {
                [D1] = new[] { new JugadorEnCancha(A, PosicionJugador.Bloqueador), new JugadorEnCancha(B, PosicionJugador.Defensor) },
                [D2] = new[] { new JugadorEnCancha(C, PosicionJugador.Bloqueador), new JugadorEnCancha(D, PosicionJugador.Defensor) }
            },
            SacadorInicialDupla1JugadorId = A,
            SacadorInicialDupla2JugadorId = C,
            DuplaQueSacaPrimeroId = duplaQueSacaPrimero,
            GanadoresRalliesPrevios = ganadores,
            AccionesRallyActual = Array.Empty<Accion>(),
            Combinadas = Array.Empty<ModificadorCombinada>()
        };

        [Fact]
        public void Rally1_D1SacaPrimero_SacaInicialD1()
        {
            Assert.Equal(A, new MotorRally().QuienSaca(Ctx(D1, Array.Empty<int>())));
        }

        [Fact]
        public void Rally2_D2GanoAnterior_SacaInicialD2()
        {
            Assert.Equal(C, new MotorRally().QuienSaca(Ctx(D1, new[] { D2 })));
        }

        [Fact]
        public void RachaD1_MismoSacador()
        {
            Assert.Equal(A, new MotorRally().QuienSaca(Ctx(D1, new[] { D1, D1 })));
        }

        [Fact]
        public void SideOut_D1Recupera_SacaCompanero()
        {
            Assert.Equal(B, new MotorRally().QuienSaca(Ctx(D1, new[] { D2, D1 })));
        }

        [Fact]
        public void DosSideOuts_D2Recupera2daVez_SacaCompaneroD2()
        {
            Assert.Equal(D, new MotorRally().QuienSaca(Ctx(D1, new[] { D2, D1, D2 })));
        }

        [Fact]
        public void Rally1_D2SacaPrimero_SacaInicialD2()
        {
            Assert.Equal(C, new MotorRally().QuienSaca(Ctx(D2, Array.Empty<int>())));
        }

        [Fact]
        public void TerceraAdquisicion_D1_VuelveAlInicial()
        {
            Assert.Equal(A, new MotorRally().QuienSaca(Ctx(D1, new[] { D2, D1, D2, D1 })));
        }
    }
}
```

## SandStats.Tests\SugerenciaPasoTests.cs
```csharp
using SandStats.Models;
using SandStats.Models.EnVivo;
using SandStats.Services.EnVivo;
using Xunit;

namespace SandStats.Tests
{
    public class SugerenciaPasoTests
    {
        private const int D1 = 1, D2 = 2;
        private const int A = 10, B = 11, C = 20, D = 21;

        private static Accion Acc(Fundamento f, Calidad? cal, int jugadorId) => new()
        {
            RallyId = 0, Secuencia = 0, JugadorId = jugadorId,
            Fundamento = f, Calidad = cal, Complejo = Complejo.K2,
            FechaHora = DateTime.UtcNow
        };

        private static ContextoRally Ctx(IReadOnlyList<Accion> acciones,
            IReadOnlyList<ModificadorCombinada>? combinadas = null) => new()
        {
            Dupla1Id = D1,
            Dupla2Id = D2,
            JugadoresPorDupla = new Dictionary<int, IReadOnlyList<JugadorEnCancha>>
            {
                [D1] = [new JugadorEnCancha(A, PosicionJugador.Bloqueador), new JugadorEnCancha(B, PosicionJugador.Defensor)],
                [D2] = [new JugadorEnCancha(C, PosicionJugador.Bloqueador), new JugadorEnCancha(D, PosicionJugador.Defensor)]
            },
            SacadorInicialDupla1JugadorId = A,
            SacadorInicialDupla2JugadorId = C,
            DuplaQueSacaPrimeroId = D1,
            GanadoresRalliesPrevios = Array.Empty<int>(),
            AccionesRallyActual = acciones,
            Combinadas = combinadas ?? Array.Empty<ModificadorCombinada>()
        };

        private static IReadOnlyList<ModificadorCombinada> CombinasBloqueoDefensa() =>
        [
            new ModificadorCombinada { Id = 7,  FundamentoCargado = Fundamento.Bloqueo, CalidadCargada = Calidad.DoblePositivo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Slash },
            new ModificadorCombinada { Id = 8,  FundamentoCargado = Fundamento.Bloqueo, CalidadCargada = Calidad.Positivo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Negativo },
            new ModificadorCombinada { Id = 9,  FundamentoCargado = Fundamento.Bloqueo, CalidadCargada = Calidad.Slash,         FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo },
            new ModificadorCombinada { Id = 10, FundamentoCargado = Fundamento.Bloqueo, CalidadCargada = Calidad.Negativo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo },
            new ModificadorCombinada { Id = 11, FundamentoCargado = Fundamento.Bloqueo, CalidadCargada = Calidad.DobleNegativo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.DoblePositivo },
            new ModificadorCombinada { Id = 12, FundamentoCargado = Fundamento.Defensa, CalidadCargada = Calidad.DoblePositivo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Negativo },
            new ModificadorCombinada { Id = 13, FundamentoCargado = Fundamento.Defensa, CalidadCargada = Calidad.Positivo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Negativo },
            new ModificadorCombinada { Id = 14, FundamentoCargado = Fundamento.Defensa, CalidadCargada = Calidad.Exclamativa,   FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo },
            new ModificadorCombinada { Id = 15, FundamentoCargado = Fundamento.Defensa, CalidadCargada = Calidad.Slash,         FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo },
            new ModificadorCombinada { Id = 16, FundamentoCargado = Fundamento.Defensa, CalidadCargada = Calidad.Negativo,      FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.Positivo },
            new ModificadorCombinada { Id = 17, FundamentoCargado = Fundamento.Defensa, CalidadCargada = Calidad.DobleNegativo, FundamentoDerivado = Fundamento.Ataque, CalidadDerivada = Calidad.DoblePositivo }
        ];

        // â”€â”€ Regla 1: rally vacÃ­o â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla1_RallyVacio_SugiereSaqueDelSacador()
        {
            var s = new MotorRally().SugerirProximoPaso(Ctx([]));

            Assert.Single(s.Opciones);
            Assert.Equal(Fundamento.Saque, s.Opciones[0].Fundamento);
            Assert.Equal(A, s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D1, s.DuplaId);
            Assert.Equal(Complejo.K2, s.Complejo);
            Assert.False(s.PermiteDe2da);
            Assert.Null(s.JugadorDe2daId);
            Assert.False(s.EsRejuego);
        }

        // â”€â”€ Regla 2: saque sin calidad â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla2_SaqueSinCalidad_SugiereRecepcionDupla2()
        {
            var s = new MotorRally().SugerirProximoPaso(Ctx([Acc(Fundamento.Saque, null, A)]));

            Assert.Single(s.Opciones);
            Assert.Equal(Fundamento.Recepcion, s.Opciones[0].Fundamento);
            Assert.Null(s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D2, s.DuplaId);
            Assert.Equal(Complejo.K1, s.Complejo);
            Assert.False(s.PermiteDe2da);
        }

        // â”€â”€ Regla 3: recepcion #/+/!/âˆ’ â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla3_RecepcionPositivo_SugiereAtaqueK1Con2da()
        {
            var acciones = new Accion[] { Acc(Fundamento.Saque, null, A), Acc(Fundamento.Recepcion, Calidad.Positivo, C) };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones));

            Assert.Single(s.Opciones);
            Assert.Equal(Fundamento.Ataque, s.Opciones[0].Fundamento);
            Assert.Equal(C, s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D2, s.DuplaId);
            Assert.Equal(Complejo.K1, s.Complejo);
            Assert.True(s.PermiteDe2da);
            Assert.Equal(D, s.JugadorDe2daId);
            Assert.False(s.EsRejuego);
        }

        // â”€â”€ Regla 4: recepcion / â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla4_RecepcionVendida_SugiereAtaqueDuplaSacadoraK2()
        {
            var acciones = new Accion[] { Acc(Fundamento.Saque, null, A), Acc(Fundamento.Recepcion, Calidad.Slash, C) };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones));

            Assert.Single(s.Opciones);
            Assert.Equal(Fundamento.Ataque, s.Opciones[0].Fundamento);
            Assert.Null(s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D1, s.DuplaId);
            Assert.Equal(Complejo.K2, s.Complejo);
            Assert.False(s.PermiteDe2da);
        }

        // â”€â”€ Regla 5: ataque sin calidad â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla5_AtaqueSinCalidad_SugiereBloqueoYDefensaRival()
        {
            var acciones = new Accion[]
            {
                Acc(Fundamento.Saque, null, A),
                Acc(Fundamento.Recepcion, Calidad.Positivo, C),
                Acc(Fundamento.Ataque, null, C)
            };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones));

            Assert.Equal(2, s.Opciones.Count);
            Assert.Equal(Fundamento.Bloqueo, s.Opciones[0].Fundamento);
            Assert.Equal(A, s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(Fundamento.Defensa, s.Opciones[1].Fundamento);
            Assert.Equal(B, s.Opciones[1].JugadorSugeridoId);
            Assert.Equal(D1, s.DuplaId);
            Assert.Equal(Complejo.K2, s.Complejo);
            Assert.False(s.PermiteDe2da);
        }

        // â”€â”€ Regla 6: bloqueo + â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla6_BloqueoPositivo_SugiereAtaqueDuplaBloqueadorK2()
        {
            var acciones = new Accion[]
            {
                Acc(Fundamento.Saque, null, A),
                Acc(Fundamento.Recepcion, Calidad.Positivo, C),
                Acc(Fundamento.Ataque, null, C),
                Acc(Fundamento.Bloqueo, Calidad.Positivo, A)
            };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones));

            Assert.Single(s.Opciones);
            Assert.Equal(Fundamento.Ataque, s.Opciones[0].Fundamento);
            Assert.Null(s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D1, s.DuplaId);
            Assert.Equal(Complejo.K2, s.Complejo);
            Assert.False(s.PermiteDe2da);
            Assert.False(s.EsRejuego);
        }

        // â”€â”€ Regla 7: bloqueo âˆ’ â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla7_BloqueoNegativo_SugiereAtacanteAnteriorK2()
        {
            var acciones = new Accion[]
            {
                Acc(Fundamento.Saque, null, A),
                Acc(Fundamento.Recepcion, Calidad.Positivo, C),
                Acc(Fundamento.Ataque, null, C),
                Acc(Fundamento.Bloqueo, Calidad.Negativo, A)
            };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones));

            Assert.Single(s.Opciones);
            Assert.Equal(Fundamento.Ataque, s.Opciones[0].Fundamento);
            Assert.Equal(C, s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D2, s.DuplaId);
            Assert.Equal(Complejo.K2, s.Complejo);
            Assert.True(s.PermiteDe2da);
            Assert.Equal(D, s.JugadorDe2daId);
            Assert.False(s.EsRejuego);
        }

        // â”€â”€ Regla 8: bloqueo / â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla8_BloqueoVendido_SugiereAtacanteAnteriorRejuego()
        {
            var acciones = new Accion[]
            {
                Acc(Fundamento.Saque, null, A),
                Acc(Fundamento.Recepcion, Calidad.Positivo, C),
                Acc(Fundamento.Ataque, null, C),
                Acc(Fundamento.Bloqueo, Calidad.Slash, A)
            };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones));

            Assert.Equal(C, s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D2, s.DuplaId);
            Assert.Equal(Complejo.K2, s.Complejo);
            Assert.True(s.PermiteDe2da);
            Assert.Equal(D, s.JugadorDe2daId);
            Assert.True(s.EsRejuego);
        }

        // â”€â”€ Regla 9: defensa #/+ â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla9_DefensaDoblePositivo_SugiereAtaqueDefensorCon2da()
        {
            var acciones = new Accion[]
            {
                Acc(Fundamento.Saque, null, A),
                Acc(Fundamento.Recepcion, Calidad.Positivo, C),
                Acc(Fundamento.Ataque, null, C),
                Acc(Fundamento.Defensa, Calidad.DoblePositivo, A)
            };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones));

            Assert.Equal(A, s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D1, s.DuplaId);
            Assert.Equal(Complejo.K2, s.Complejo);
            Assert.True(s.PermiteDe2da);
            Assert.Equal(B, s.JugadorDe2daId);
            Assert.False(s.EsRejuego);
        }

        // â”€â”€ Regla 10: defensa ! (cobertura) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla10_DefensaCobertura_SugiereAtacanteAnterior()
        {
            // D cubre el bloqueo que volviÃ³ sobre el ataque de su compaÃ±ero C
            var acciones = new Accion[]
            {
                Acc(Fundamento.Saque, null, A),
                Acc(Fundamento.Recepcion, Calidad.Positivo, C),
                Acc(Fundamento.Ataque, null, C),
                Acc(Fundamento.Defensa, Calidad.Exclamativa, D)
            };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones));

            Assert.Equal(C, s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D2, s.DuplaId);
            Assert.Equal(Complejo.K2, s.Complejo);
            Assert.True(s.PermiteDe2da);
            Assert.Equal(D, s.JugadorDe2daId);
            Assert.False(s.EsRejuego);
        }

        // â”€â”€ Regla 11: defensa âˆ’ â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla11_DefensaNegativo_SugiereAtacanteAnteriorK2()
        {
            var acciones = new Accion[]
            {
                Acc(Fundamento.Saque, null, A),
                Acc(Fundamento.Recepcion, Calidad.Positivo, C),
                Acc(Fundamento.Ataque, null, C),
                Acc(Fundamento.Defensa, Calidad.Negativo, A)
            };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones));

            Assert.Equal(C, s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D2, s.DuplaId);
            Assert.Equal(Complejo.K2, s.Complejo);
            Assert.True(s.PermiteDe2da);
            Assert.Equal(D, s.JugadorDe2daId);
            Assert.False(s.EsRejuego);
        }

        // â”€â”€ Regla 11b: defensa / â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla11b_DefensaVendida_SugiereAtacanteAnteriorK2()
        {
            var acciones = new Accion[]
            {
                Acc(Fundamento.Saque, null, A),
                Acc(Fundamento.Recepcion, Calidad.Positivo, C),
                Acc(Fundamento.Ataque, null, C),
                Acc(Fundamento.Defensa, Calidad.Slash, A)
            };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones));

            Assert.Equal(C, s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D2, s.DuplaId);
            Assert.Equal(Complejo.K2, s.Complejo);
            Assert.True(s.PermiteDe2da);
            Assert.Equal(D, s.JugadorDe2daId);
            Assert.False(s.EsRejuego);
        }

        // â”€â”€ Regla 13: armado âˆ’ â†’ rival ataca K2 â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla13_ArmadoNegativo_SugiereAtaqueRival()
        {
            // D2 recibiÃ³ y su compaÃ±ero D armÃ³ mal (free ball a D1)
            var acciones = new Accion[]
            {
                Acc(Fundamento.Saque,     null,             A),
                Acc(Fundamento.Recepcion, Calidad.Positivo, C),
                Acc(Fundamento.Armado,    Calidad.Negativo, D)
            };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones));

            Assert.Single(s.Opciones);
            Assert.Equal(Fundamento.Ataque, s.Opciones[0].Fundamento);
            Assert.Null(s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D1, s.DuplaId);
            Assert.Equal(Complejo.K2, s.Complejo);
            Assert.False(s.PermiteDe2da);
            Assert.Null(s.JugadorDe2daId);
            Assert.False(s.EsRejuego);
        }

        // â”€â”€ Regla 14: free ball â†’ rival ataca K2 â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla14_FreeBall_SugiereAtaqueRival()
        {
            // D2 recibiÃ³; el compaÃ±ero D marcÃ³ free ball (por toque anterior malo)
            var acciones = new Accion[]
            {
                Acc(Fundamento.Saque,     null,             A),
                Acc(Fundamento.Recepcion, Calidad.Positivo, C),
                Acc(Fundamento.FreeBall,  null,             D)
            };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones));

            Assert.Single(s.Opciones);
            Assert.Equal(Fundamento.Ataque, s.Opciones[0].Fundamento);
            Assert.Null(s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D1, s.DuplaId);
            Assert.Equal(Complejo.K2, s.Complejo);
            Assert.False(s.PermiteDe2da);
            Assert.Null(s.JugadorDe2daId);
            Assert.False(s.EsRejuego);
        }

        // â”€â”€ R5: CalidadesValidas por fundamento â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void R5_CalidadesValidas_Bloqueo_NoIncluyeExclamativa()
        {
            var acciones = new Accion[] { Acc(Fundamento.Ataque, null, C) };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones, CombinasBloqueoDefensa()));

            var opc = s.Opciones.First(o => o.Fundamento == Fundamento.Bloqueo);
            Assert.NotNull(opc.CalidadesValidas);
            Assert.Contains(Calidad.DoblePositivo, opc.CalidadesValidas);
            Assert.Contains(Calidad.Positivo,      opc.CalidadesValidas);
            Assert.Contains(Calidad.Slash,         opc.CalidadesValidas);
            Assert.Contains(Calidad.Negativo,      opc.CalidadesValidas);
            Assert.Contains(Calidad.DobleNegativo, opc.CalidadesValidas);
            Assert.DoesNotContain(Calidad.Exclamativa, opc.CalidadesValidas);
        }

        [Fact]
        public void R5_CalidadesValidas_Defensa_IncluyeExclamativa()
        {
            var acciones = new Accion[] { Acc(Fundamento.Ataque, null, C) };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones, CombinasBloqueoDefensa()));

            var opc = s.Opciones.First(o => o.Fundamento == Fundamento.Defensa);
            Assert.NotNull(opc.CalidadesValidas);
            Assert.Contains(Calidad.Exclamativa, opc.CalidadesValidas);
            Assert.Equal(6, opc.CalidadesValidas.Count);
        }

        // â”€â”€ Regla 12: transversal K1â†’K2 (segundo ataque del rally) â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€

        [Fact]
        public void Regla12_SegundoAtaqueDuplaReceptora_EsK2()
        {
            // Secuencia: D1 saca â†’ D2 recibe â†’ D2 ataca (K1) â†’ D1 bloquea mal (âˆ’)
            // â†’ D2 (receptora) vuelve a atacar: debe sugerir K2, no K1
            var acciones = new Accion[]
            {
                Acc(Fundamento.Saque,    null,            A),
                Acc(Fundamento.Recepcion, Calidad.Positivo, C),
                Acc(Fundamento.Ataque,   null,            C),
                Acc(Fundamento.Bloqueo,  Calidad.Negativo, A)
            };
            var s = new MotorRally().SugerirProximoPaso(Ctx(acciones));

            Assert.Equal(C, s.Opciones[0].JugadorSugeridoId);
            Assert.Equal(D2, s.DuplaId);
            Assert.Equal(Complejo.K2, s.Complejo);
        }
    }
}
```

## SandStats.Tests\TestAuthHandler.cs
```csharp
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SandStats.Tests
{
    public class TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "testuser") };
            var identity = new ClaimsIdentity(claims, "Test");
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), "Test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
```

## Migrations\20260624181647_AgregaModuloEnVivo.cs
```csharp
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class AgregaModuloEnVivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartidosEnVivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Torneo = table.Column<string>(type: "TEXT", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Dupla1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Dupla2Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidosEnVivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartidosEnVivo_Duplas_Dupla1Id",
                        column: x => x.Dupla1Id,
                        principalTable: "Duplas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartidosEnVivo_Duplas_Dupla2Id",
                        column: x => x.Dupla2Id,
                        principalTable: "Duplas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SetsEnVivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartidoEnVivoId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeroSet = table.Column<int>(type: "INTEGER", nullable: false),
                    SacadorInicialJugadorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetsEnVivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetsEnVivo_Jugadores_SacadorInicialJugadorId",
                        column: x => x.SacadorInicialJugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SetsEnVivo_PartidosEnVivo_PartidoEnVivoId",
                        column: x => x.PartidoEnVivoId,
                        principalTable: "PartidosEnVivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rallies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SetEnVivoId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeroRally = table.Column<int>(type: "INTEGER", nullable: false),
                    DuplaGanadoraId = table.Column<int>(type: "INTEGER", nullable: true),
                    MarcadorDupla1 = table.Column<int>(type: "INTEGER", nullable: false),
                    MarcadorDupla2 = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rallies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rallies_Duplas_DuplaGanadoraId",
                        column: x => x.DuplaGanadoraId,
                        principalTable: "Duplas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rallies_SetsEnVivo_SetEnVivoId",
                        column: x => x.SetEnVivoId,
                        principalTable: "SetsEnVivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Acciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RallyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Secuencia = table.Column<int>(type: "INTEGER", nullable: false),
                    JugadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Fundamento = table.Column<int>(type: "INTEGER", nullable: false),
                    Calidad = table.Column<int>(type: "INTEGER", nullable: false),
                    EsRejuego = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Acciones_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Acciones_Rallies_RallyId",
                        column: x => x.RallyId,
                        principalTable: "Rallies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesAtaque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Lado = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoAccion = table.Column<int>(type: "INTEGER", nullable: false),
                    ZonaDestino = table.Column<int>(type: "INTEGER", nullable: false),
                    EsVarilla = table.Column<bool>(type: "INTEGER", nullable: false),
                    EsEspecial = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesAtaque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesAtaque_Acciones_AccionId",
                        column: x => x.AccionId,
                        principalTable: "Acciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesRecepcion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccionId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoRecepcion = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesRecepcion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesRecepcion_Acciones_AccionId",
                        column: x => x.AccionId,
                        principalTable: "Acciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesSaque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ZonaSaque = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoSaque = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesSaque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesSaque_Acciones_AccionId",
                        column: x => x.AccionId,
                        principalTable: "Acciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acciones_JugadorId",
                table: "Acciones",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Acciones_RallyId",
                table: "Acciones",
                column: "RallyId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesAtaque_AccionId",
                table: "DetallesAtaque",
                column: "AccionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesRecepcion_AccionId",
                table: "DetallesRecepcion",
                column: "AccionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesSaque_AccionId",
                table: "DetallesSaque",
                column: "AccionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartidosEnVivo_Dupla1Id",
                table: "PartidosEnVivo",
                column: "Dupla1Id");

            migrationBuilder.CreateIndex(
                name: "IX_PartidosEnVivo_Dupla2Id",
                table: "PartidosEnVivo",
                column: "Dupla2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Rallies_DuplaGanadoraId",
                table: "Rallies",
                column: "DuplaGanadoraId");

            migrationBuilder.CreateIndex(
                name: "IX_Rallies_SetEnVivoId",
                table: "Rallies",
                column: "SetEnVivoId");

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_PartidoEnVivoId",
                table: "SetsEnVivo",
                column: "PartidoEnVivoId");

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_SacadorInicialJugadorId",
                table: "SetsEnVivo",
                column: "SacadorInicialJugadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesAtaque");

            migrationBuilder.DropTable(
                name: "DetallesRecepcion");

            migrationBuilder.DropTable(
                name: "DetallesSaque");

            migrationBuilder.DropTable(
                name: "Acciones");

            migrationBuilder.DropTable(
                name: "Rallies");

            migrationBuilder.DropTable(
                name: "SetsEnVivo");

            migrationBuilder.DropTable(
                name: "PartidosEnVivo");
        }
    }
}
```

## Migrations\20260704224023_AjustesLogicaRally.cs
```csharp
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class AjustesLogicaRally : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetsEnVivo_Jugadores_SacadorInicialJugadorId",
                table: "SetsEnVivo");

            migrationBuilder.RenameColumn(
                name: "SacadorInicialJugadorId",
                table: "SetsEnVivo",
                newName: "SacadorInicialDupla2JugadorId");

            migrationBuilder.RenameIndex(
                name: "IX_SetsEnVivo_SacadorInicialJugadorId",
                table: "SetsEnVivo",
                newName: "IX_SetsEnVivo_SacadorInicialDupla2JugadorId");

            migrationBuilder.AddColumn<int>(
                name: "SacadorInicialDupla1JugadorId",
                table: "SetsEnVivo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Complejo",
                table: "Acciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EsDe2da",
                table: "Acciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ModificadoresCombinadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FundamentoCargado = table.Column<int>(type: "INTEGER", nullable: false),
                    CalidadCargada = table.Column<int>(type: "INTEGER", nullable: false),
                    FundamentoDerivado = table.Column<int>(type: "INTEGER", nullable: false),
                    CalidadDerivada = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModificadoresCombinadas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_SacadorInicialDupla1JugadorId",
                table: "SetsEnVivo",
                column: "SacadorInicialDupla1JugadorId");

            migrationBuilder.CreateIndex(
                name: "UX_ModificadorCombinada",
                table: "ModificadoresCombinadas",
                columns: new[] { "FundamentoCargado", "CalidadCargada", "FundamentoDerivado" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SetsEnVivo_Jugadores_SacadorInicialDupla1JugadorId",
                table: "SetsEnVivo",
                column: "SacadorInicialDupla1JugadorId",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SetsEnVivo_Jugadores_SacadorInicialDupla2JugadorId",
                table: "SetsEnVivo",
                column: "SacadorInicialDupla2JugadorId",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetsEnVivo_Jugadores_SacadorInicialDupla1JugadorId",
                table: "SetsEnVivo");

            migrationBuilder.DropForeignKey(
                name: "FK_SetsEnVivo_Jugadores_SacadorInicialDupla2JugadorId",
                table: "SetsEnVivo");

            migrationBuilder.DropTable(
                name: "ModificadoresCombinadas");

            migrationBuilder.DropIndex(
                name: "IX_SetsEnVivo_SacadorInicialDupla1JugadorId",
                table: "SetsEnVivo");

            migrationBuilder.DropColumn(
                name: "SacadorInicialDupla1JugadorId",
                table: "SetsEnVivo");

            migrationBuilder.DropColumn(
                name: "Complejo",
                table: "Acciones");

            migrationBuilder.DropColumn(
                name: "EsDe2da",
                table: "Acciones");

            migrationBuilder.RenameColumn(
                name: "SacadorInicialDupla2JugadorId",
                table: "SetsEnVivo",
                newName: "SacadorInicialJugadorId");

            migrationBuilder.RenameIndex(
                name: "IX_SetsEnVivo_SacadorInicialDupla2JugadorId",
                table: "SetsEnVivo",
                newName: "IX_SetsEnVivo_SacadorInicialJugadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SetsEnVivo_Jugadores_SacadorInicialJugadorId",
                table: "SetsEnVivo",
                column: "SacadorInicialJugadorId",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
```

## Migrations\20260706152453_SeedCombinadas.cs
```csharp
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class SeedCombinadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ModificadoresCombinadas",
                columns: new[] { "Id", "CalidadCargada", "CalidadDerivada", "FundamentoCargado", "FundamentoDerivado" },
                values: new object[,]
                {
                    { 1, 5, 1, 1, 0 },
                    { 2, 4, 1, 1, 0 },
                    { 3, 3, 3, 1, 0 },
                    { 4, 2, 2, 1, 0 },
                    { 5, 1, 4, 1, 0 },
                    { 6, 0, 5, 1, 0 },
                    { 7, 5, 2, 3, 2 },
                    { 8, 4, 1, 3, 2 },
                    { 9, 2, 3, 3, 2 },
                    { 10, 1, 4, 3, 2 },
                    { 11, 0, 5, 3, 2 },
                    { 12, 5, 1, 4, 2 },
                    { 13, 4, 1, 4, 2 },
                    { 14, 3, 3, 4, 2 },
                    { 15, 2, 4, 4, 2 },
                    { 16, 1, 4, 4, 2 },
                    { 17, 0, 5, 4, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 17);
        }
    }
}
```

## Migrations\20260708012515_AgregaDuplaSacaPrimero.cs
```csharp
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class AgregaDuplaSacaPrimero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuplaQueSacaPrimeroId",
                table: "SetsEnVivo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_DuplaQueSacaPrimeroId",
                table: "SetsEnVivo",
                column: "DuplaQueSacaPrimeroId");

            migrationBuilder.AddForeignKey(
                name: "FK_SetsEnVivo_Duplas_DuplaQueSacaPrimeroId",
                table: "SetsEnVivo",
                column: "DuplaQueSacaPrimeroId",
                principalTable: "Duplas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetsEnVivo_Duplas_DuplaQueSacaPrimeroId",
                table: "SetsEnVivo");

            migrationBuilder.DropIndex(
                name: "IX_SetsEnVivo_DuplaQueSacaPrimeroId",
                table: "SetsEnVivo");

            migrationBuilder.DropColumn(
                name: "DuplaQueSacaPrimeroId",
                table: "SetsEnVivo");
        }
    }
}
```

## Migrations\20260708015517_CalidadNullable.cs
```csharp
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class CalidadNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Calidad",
                table: "Acciones",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Calidad",
                table: "Acciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }
    }
}
```

## Migrations\20260711040624_CoberturaDerivaPositivo.cs
```csharp
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class CoberturaDerivaPositivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 9,
                column: "CalidadDerivada",
                value: 4);

            migrationBuilder.UpdateData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 14,
                column: "CalidadDerivada",
                value: 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 9,
                column: "CalidadDerivada",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 14,
                column: "CalidadDerivada",
                value: 3);
        }
    }
}
```

## Migrations\20260712185919_CierreDeSetYTipoCierre.cs
```csharp
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class CierreDeSetYTipoCierre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuplaGanadoraId",
                table: "SetsEnVivo",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoCierre",
                table: "Rallies",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_DuplaGanadoraId",
                table: "SetsEnVivo",
                column: "DuplaGanadoraId");

            migrationBuilder.AddForeignKey(
                name: "FK_SetsEnVivo_Duplas_DuplaGanadoraId",
                table: "SetsEnVivo",
                column: "DuplaGanadoraId",
                principalTable: "Duplas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetsEnVivo_Duplas_DuplaGanadoraId",
                table: "SetsEnVivo");

            migrationBuilder.DropIndex(
                name: "IX_SetsEnVivo_DuplaGanadoraId",
                table: "SetsEnVivo");

            migrationBuilder.DropColumn(
                name: "DuplaGanadoraId",
                table: "SetsEnVivo");

            migrationBuilder.DropColumn(
                name: "TipoCierre",
                table: "Rallies");
        }
    }
}
```
