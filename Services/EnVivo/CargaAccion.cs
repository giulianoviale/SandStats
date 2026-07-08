using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public record CargaAccion(Fundamento Fundamento, Calidad? Calidad, int JugadorId, bool EsDe2da);
}
