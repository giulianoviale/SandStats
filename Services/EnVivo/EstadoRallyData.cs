using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public record EstadoRallyData(
        Rally Rally,
        IReadOnlyList<Accion> Acciones,
        SugerenciaPaso? Sugerencia,
        ResultadoMarcador Marcador,
        IReadOnlyDictionary<int, string> NombresJugadores
    );
}
