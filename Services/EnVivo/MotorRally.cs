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
