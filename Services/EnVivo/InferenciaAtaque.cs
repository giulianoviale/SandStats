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
            // Tabla extraída de CargarAtaques.cshtml.cs (página legacy sin modificar)
            int[] tl = (lado, esRol4) switch
            {
                (TipoLado.Bueno, true)  => [1, 9, 2],
                (TipoLado.Atras, true)  => [5, 7, 4],
                (TipoLado.Medio, true)  => [1, 9, 2],
                (TipoLado.Bueno, false) => [5, 7, 4],   // Rol2 Bueno = Rol4 Atrás
                (TipoLado.Atras, false) => [1, 9, 2],   // Rol2 Atrás = Rol4 Bueno
                (TipoLado.Medio, false) => [5, 7, 4],
                _                       => []
            };
            return tl.Contains(n);
        }
    }
}
