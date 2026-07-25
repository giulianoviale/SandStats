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
                        $"no es una combinación válida");
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

                // Regla 7: bloqueo − → mismo atacante previo, K2, permite 2da
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

                // Regla 8: bloqueo / → mismo atacante previo, K2, rejuego, permite 2da
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

                // Regla 10: defensa ! (cobertura) → mismo atacante previo, K2, permite 2da
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

                // Regla 11: defensa −/ → atacante previo, K2, permite 2da
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

            // Regla 13: armado − → rival ataca, free ball K2
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

            // Regla 14: free ball (marcador de posesión, Calidad null) → rival ataca K2
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
