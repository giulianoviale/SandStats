using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public record Derivacion(Fundamento FundamentoDerivado, Calidad CalidadDerivada);
    public record Cierre(int DuplaGanadoraId);
    public record ResultadoRegistro(Derivacion? Derivacion, Cierre? Cierre);
}
