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
            Combinadas = Array.Empty<ModificadorCombinada>()
        };

        // ── Cierres por carga ────────────────────────────────────────────────

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

        // ── Cierres directos ────────────────────────────────────────────────

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

        // ── Continuaciones (Cierre == null) ──────────────────────────────────

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

        // ── Ataque pendiente de derivación ───────────────────────────────────

        [Fact]
        public void Ataque_CalidadNull_NiDerivaНiCierra()
        {
            var r = new MotorRally().Registrar(Ctx(), new CargaAccion(Fundamento.Ataque, null, A, false));
            Assert.Null(r.Derivacion);
            Assert.Null(r.Cierre);
        }
    }
}
