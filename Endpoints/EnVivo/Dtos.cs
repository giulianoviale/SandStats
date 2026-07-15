using SandStats.Models;
using SandStats.Models.EnVivo;
using SandStats.Models.SandStats.Models;
using SandStats.Services.EnVivo;

namespace SandStats.Endpoints.EnVivo
{
    // ── Requests ────────────────────────────────────────────────────────────────

    public record CrearPartidoRequest(int Dupla1Id, int Dupla2Id, string Torneo, DateTime Fecha);

    public record IniciarSetRequest(
        int NumeroSet,
        int SacadorInicialD1Id,
        int SacadorInicialD2Id,
        int DuplaQueSacaPrimeroId);

    // Flat: cubre DetalleSaque, DetalleRecepcion y DetalleAtaque; el handler valida coherencia.
    public record DetalleAccionRequest(
        TipoLado? Lado,
        TipoAcciones? TipoAccion,
        ZonaCancha? ZonaDestino,
        bool? EsVarilla,
        bool? EsEspecial,
        ZonaSaque? ZonaSaque,
        TipoSaque? TipoSaque,
        TipoRecepcion? TipoRecepcion);

    public record RegistrarAccionRequest(
        Fundamento Fundamento,
        Calidad? Calidad,
        int JugadorId,
        bool EsDe2da,
        DetalleAccionRequest? Detalle);

    public record CierreDirectoRequest(TipoCierreDirecto Tipo, int? DuplaGanadoraId);

    public record CerrarSetRequest(int DuplaGanadoraId);

    // ── Responses ───────────────────────────────────────────────────────────────

    public record PartidoResponse(int Id, int Dupla1Id, int Dupla2Id, string Torneo, DateTime Fecha);

    public record SetIniciadoResponse(int Id, int PartidoId, int NumeroSet);

    public record RallyAbiertoResponse(int Id, int NumeroRally, int MarcadorDupla1, int MarcadorDupla2);

    public record AccionResponse(
        int Secuencia,
        int JugadorId,
        string JugadorNombre,
        Fundamento Fundamento,
        Calidad? Calidad,
        Complejo Complejo,
        bool EsRejuego);

    public record OpcionPasoResponse(Fundamento Fundamento, int? JugadorSugeridoId, string? JugadorNombre);

    public record SugerenciaPasoResponse(
        IReadOnlyList<OpcionPasoResponse> Opciones,
        int DuplaId,
        Complejo Complejo,
        bool PermiteDe2da,
        int? JugadorDe2daId,
        bool EsRejuego);

    public record ResultadoMarcadorResponse(
        bool SetTerminado,
        bool PartidoTerminado,
        bool CambioDeLado,
        int? DuplaGanadoraSetId);

    public record EstadoRallyResponse(
        int RallyId,
        int NumeroRally,
        int MarcadorDupla1,
        int MarcadorDupla2,
        IReadOnlyList<AccionResponse> Acciones,
        SugerenciaPasoResponse? Sugerencia,
        ResultadoMarcadorResponse Marcador,
        bool Cerrado,
        int? DuplaGanadoraId,
        TipoCierreRally? TipoCierre);
}
