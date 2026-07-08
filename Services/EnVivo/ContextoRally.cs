using SandStats.Models.EnVivo;

namespace SandStats.Services.EnVivo
{
    public class ContextoRally
    {
        public required int Dupla1Id { get; init; }
        public required int Dupla2Id { get; init; }
        public required IReadOnlyDictionary<int, IReadOnlyList<JugadorEnCancha>> JugadoresPorDupla { get; init; }
        public required int SacadorInicialDupla1JugadorId { get; init; }
        public required int SacadorInicialDupla2JugadorId { get; init; }
        public required int DuplaQueSacaPrimeroId { get; init; }
        public required IReadOnlyList<int> GanadoresRalliesPrevios { get; init; }
        public required IReadOnlyList<Accion> AccionesRallyActual { get; init; }
        public required IReadOnlyList<ModificadorCombinada> Combinadas { get; init; }
    }
}
