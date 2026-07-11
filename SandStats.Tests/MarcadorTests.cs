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

        // ── Set termina ──────────────────────────────────────────────────────

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

        // ── Partido termina ──────────────────────────────────────────────────

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

        // ── Cambio de lado ───────────────────────────────────────────────────

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
            // total=35=7x5, pero el set cerró en ese punto → CambioDeLado=false
            var r = Eval(21, 14);
            Assert.True(r.SetTerminado);
            Assert.False(r.CambioDeLado);
        }
    }
}
