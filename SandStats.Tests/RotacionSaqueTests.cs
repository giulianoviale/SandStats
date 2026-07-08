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
