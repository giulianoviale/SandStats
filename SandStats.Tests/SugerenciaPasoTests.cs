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

        private static ContextoRally Ctx(IReadOnlyList<Accion> acciones) => new()
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
            Combinadas = Array.Empty<ModificadorCombinada>()
        };

        // ── Regla 1: rally vacío ─────────────────────────────────────────────

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

        // ── Regla 2: saque sin calidad ───────────────────────────────────────

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

        // ── Regla 3: recepcion #/+/!/− ───────────────────────────────────────

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

        // ── Regla 4: recepcion / ─────────────────────────────────────────────

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

        // ── Regla 5: ataque sin calidad ──────────────────────────────────────

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

        // ── Regla 6: bloqueo + ───────────────────────────────────────────────

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

        // ── Regla 7: bloqueo − ───────────────────────────────────────────────

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
            Assert.False(s.EsRejuego);
        }

        // ── Regla 8: bloqueo / ───────────────────────────────────────────────

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
            Assert.True(s.EsRejuego);
        }

        // ── Regla 9: defensa #/+ ─────────────────────────────────────────────

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

        // ── Regla 10: defensa ! (cobertura) ─────────────────────────────────

        [Fact]
        public void Regla10_DefensaCobertura_SugiereAtacanteAnterior()
        {
            // D cubre el bloqueo que volvió sobre el ataque de su compañero C
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
            Assert.False(s.PermiteDe2da);
            Assert.False(s.EsRejuego);
        }

        // ── Regla 11: defensa − ──────────────────────────────────────────────

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
            Assert.False(s.EsRejuego);
        }

        // ── Regla 11b: defensa / ─────────────────────────────────────────────

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
            Assert.False(s.EsRejuego);
        }

        // ── Regla 12: transversal K1→K2 (segundo ataque del rally) ──────────

        [Fact]
        public void Regla12_SegundoAtaqueDuplaReceptora_EsK2()
        {
            // Secuencia: D1 saca → D2 recibe → D2 ataca (K1) → D1 bloquea mal (−)
            // → D2 (receptora) vuelve a atacar: debe sugerir K2, no K1
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
