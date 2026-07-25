using SandStats.Models;
using SandStats.Models.EnVivo;
using SandStats.Services.EnVivo;
using Xunit;

namespace SandStats.Tests
{
    public class InferenciaAtaqueTests
    {
        // ── Spike siempre devuelve Atq{n} ───────────────────────────────────

        [Fact]
        public void Spike_DevuelveAtqN_IndependienteDeLadoYRol()
        {
            Assert.Equal(TipoAcciones.Atq4,
                InferenciaAtaque.Inferir("Spike", TipoLado.Bueno, ZonaCancha.Zona4, RolJugador.Rol4));
        }

        // ── Tip Rol4 Bueno: línea = 1,9,2 ───────────────────────────────────

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

        // ── Tip Rol4 Atrás: línea = 5,7,4 ───────────────────────────────────

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

        // ── Tip Rol2 Bueno (= Rol4 Atrás): línea = 5,7,4 ───────────────────

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

        // ── Tip Rol2 Atrás (= Rol4 Bueno): línea = 1,9,2 ───────────────────

        [Fact]
        public void Tip_Rol2_Atras_Zona1_EsLinea()
        {
            Assert.Equal(TipoAcciones.Tl1,
                InferenciaAtaque.Inferir("Tip", TipoLado.Atras, ZonaCancha.Zona1, RolJugador.Rol2));
        }
    }
}
