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
