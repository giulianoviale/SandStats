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
            // Regla 1: rally vacío → saque
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

            // Regla 2: saque sin calidad → recepcion de la dupla rival
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

                // Regla 4: vendida → dupla sacadora ataca, K2
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

                // Regla 3: #/+/!/− → dupla receptora ataca
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

            // Regla 5: ataque sin calidad → bloqueo y defensa del rival
            if (ultima.Fundamento == Fundamento.Ataque && ultima.Calidad == null)
            {
                int duplaRival = RivalDe(ctx, DuplaDeJugador(ctx, ultima.JugadorId));
                var jugadoresRival = ctx.JugadoresPorDupla[duplaRival];
                int bloqueadorId = jugadoresRival.First(j => j.Posicion == PosicionJugador.Bloqueador).JugadorId;
                int defensorId   = jugadoresRival.First(j => j.Posicion == PosicionJugador.Defensor).JugadorId;
                return new SugerenciaPaso(
                    Opciones: [new OpcionPaso(Fundamento.Bloqueo, bloqueadorId), new OpcionPaso(Fundamento.Defensa, defensorId)],
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
                // Regla 6: bloqueo + → dupla del bloqueador ataca, K2
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

                // Regla 7: bloqueo − → mismo atacante previo, K2
                if (ultima.Calidad == Calidad.Negativo)
                {
                    var atacantePrevio = UltimaAccionConFundamento(ctx, Fundamento.Ataque);
                    return new SugerenciaPaso(
                        Opciones: [new OpcionPaso(Fundamento.Ataque, atacantePrevio.JugadorId)],
                        DuplaId: DuplaDeJugador(ctx, atacantePrevio.JugadorId),
                        Complejo: Complejo.K2,
                        PermiteDe2da: false,
                        JugadorDe2daId: null,
                        EsRejuego: false
                    );
                }

                // Regla 8: bloqueo / → mismo atacante previo, K2, rejuego
                if (ultima.Calidad == Calidad.Slash)
                {
                    var atacantePrevio = UltimaAccionConFundamento(ctx, Fundamento.Ataque);
                    return new SugerenciaPaso(
                        Opciones: [new OpcionPaso(Fundamento.Ataque, atacantePrevio.JugadorId)],
                        DuplaId: DuplaDeJugador(ctx, atacantePrevio.JugadorId),
                        Complejo: Complejo.K2,
                        PermiteDe2da: false,
                        JugadorDe2daId: null,
                        EsRejuego: true
                    );
                }
            }

            // Reglas 9, 10, 11: defensa
            if (ultima.Fundamento == Fundamento.Defensa)
            {
                // Regla 9: defensa #/+ → dupla del defensor ataca, K2, permite 2da
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

                // Regla 10: defensa ! (cobertura) → mismo atacante previo, K2
                if (ultima.Calidad == Calidad.Exclamativa)
                {
                    var atacantePrevio = UltimaAccionConFundamento(ctx, Fundamento.Ataque);
                    return new SugerenciaPaso(
                        Opciones: [new OpcionPaso(Fundamento.Ataque, atacantePrevio.JugadorId)],
                        DuplaId: DuplaDeJugador(ctx, atacantePrevio.JugadorId),
                        Complejo: Complejo.K2,
                        PermiteDe2da: false,
                        JugadorDe2daId: null,
                        EsRejuego: false
                    );
                }

                // Regla 11: defensa −/ → atacante previo, K2 (pelota vuelve como free ball o vendida)
                if (ultima.Calidad == Calidad.Negativo || ultima.Calidad == Calidad.Slash)
                {
                    var atacantePrevio = UltimaAccionConFundamento(ctx, Fundamento.Ataque);
                    return new SugerenciaPaso(
                        Opciones: [new OpcionPaso(Fundamento.Ataque, atacantePrevio.JugadorId)],
                        DuplaId: DuplaDeJugador(ctx, atacantePrevio.JugadorId),
                        Complejo: Complejo.K2,
                        PermiteDe2da: false,
                        JugadorDe2daId: null,
                        EsRejuego: false
                    );
                }
            }

            throw new InvalidOperationException($"Estado no manejado: {ultima.Fundamento}/{ultima.Calidad}");
        }

        private static int Companero(ContextoRally ctx, int jugadorId)
        {
            int dupla = DuplaDeJugador(ctx, jugadorId);
            return ctx.JugadoresPorDupla[dupla].First(j => j.JugadorId != jugadorId).JugadorId;
        }

        private static Accion UltimaAccionConFundamento(ContextoRally ctx, Fundamento f)
            => ctx.AccionesRallyActual.Last(a => a.Fundamento == f);

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
