using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public record SugerenciaPaso(
        IReadOnlyList<OpcionPaso> Opciones,
        int DuplaId,
        Complejo Complejo,
        bool PermiteDe2da,
        int? JugadorDe2daId,
        bool EsRejuego
    );
}
