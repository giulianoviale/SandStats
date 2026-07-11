namespace SandStats.Services.EnVivo
{
    public record ResultadoMarcador(
        bool SetTerminado,
        bool PartidoTerminado,
        bool CambioDeLado,
        int? DuplaGanadoraSetId
    );
}
