using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using SandStats.Models.EnVivo;
using SandStats.Models.SandStats.Models;
using SandStats.Services.EnVivo;

namespace SandStats.Endpoints.EnVivo
{
    public static class EnVivoEndpoints
    {
        // CSRF mitigado por SameSite=Lax (decisión consciente): la UI es same-origin y el estado
        // mutable queda protegido por la cookie de Identity. Sin antiforgery en el grupo API.
        public static void MapEnVivoEndpoints(this IEndpointRouteBuilder app)
        {
            var g = app.MapGroup("/api/envivo").RequireAuthorization();

            g.MapPost("/partidos", async (CrearPartidoRequest req, CargaEnVivoService svc) =>
            {
                try
                {
                    var p = await svc.CrearPartidoAsync(req.Dupla1Id, req.Dupla2Id, req.Torneo, req.Fecha);
                    return Results.Created($"/api/envivo/partidos/{p.Id}",
                        new PartidoResponse(p.Id, p.Dupla1Id, p.Dupla2Id, p.Torneo, p.Fecha));
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapPost("/partidos/{id:int}/sets", async (int id, IniciarSetRequest req, CargaEnVivoService svc) =>
            {
                try
                {
                    var s = await svc.IniciarSetAsync(
                        id, req.NumeroSet, req.SacadorInicialD1Id, req.SacadorInicialD2Id, req.DuplaQueSacaPrimeroId);
                    return Results.Created($"/api/envivo/sets/{s.Id}",
                        new SetIniciadoResponse(s.Id, s.PartidoEnVivoId, s.NumeroSet));
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapPost("/sets/{id:int}/rallies", async (int id, CargaEnVivoService svc) =>
            {
                try
                {
                    var r = await svc.AbrirRallyAsync(id);
                    return Results.Created($"/api/envivo/rallies/{r.Id}",
                        new RallyAbiertoResponse(r.Id, r.NumeroRally, r.MarcadorDupla1, r.MarcadorDupla2));
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapPost("/rallies/{id:int}/acciones", async (int id, RegistrarAccionRequest req, CargaEnVivoService svc) =>
            {
                try
                {
                    var detalle = MapDetalle(req.Fundamento, req.Detalle);
                    await svc.RegistrarAccionAsync(id, new CargaAccion(req.Fundamento, req.Calidad, req.JugadorId, req.EsDe2da), detalle);
                    return Results.Ok(MapEstado(await svc.ObtenerEstadoRallyAsync(id)));
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapPost("/rallies/{id:int}/cierre-directo", async (int id, CierreDirectoRequest req, CargaEnVivoService svc) =>
            {
                try
                {
                    await svc.CierreDirectoAsync(id, req.Tipo, req.DuplaGanadoraId);
                    return Results.Ok(MapEstado(await svc.ObtenerEstadoRallyAsync(id)));
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapPost("/rallies/{id:int}/deshacer", async (int id, CargaEnVivoService svc) =>
            {
                try
                {
                    await svc.DeshacerUltimaAccionAsync(id);
                    return Results.Ok(MapEstado(await svc.ObtenerEstadoRallyAsync(id)));
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapPost("/sets/{id:int}/cerrar", async (int id, CerrarSetRequest req, CargaEnVivoService svc) =>
            {
                try
                {
                    await svc.CerrarSetAsync(id, req.DuplaGanadoraId);
                    return Results.NoContent();
                }
                catch (Exception ex) { return MapError(ex); }
            });

            g.MapGet("/sets/{id:int}/estado", async (int id, CargaEnVivoService svc, ApplicationDbContext db) =>
            {
                try
                {
                    var setExiste = await db.SetsEnVivo.AnyAsync(s => s.Id == id);
                    if (!setExiste)
                        return Results.Problem($"Set {id} no encontrado", statusCode: 404);

                    var rallyId = await db.Rallies
                        .Where(r => r.SetEnVivoId == id)
                        .OrderByDescending(r => r.NumeroRally)
                        .Select(r => (int?)r.Id)
                        .FirstOrDefaultAsync();

                    if (rallyId == null)
                    {
                        // Set recién iniciado sin rallies: estado vacío con cerrado=true para mostrar "Abrir rally"
                        return Results.Ok(new EstadoRallyResponse(
                            0, 0, 0, 0,
                            Array.Empty<AccionResponse>(),
                            null,
                            new ResultadoMarcadorResponse(false, false, false, null),
                            Cerrado: true,
                            null, null));
                    }

                    return Results.Ok(MapEstado(await svc.ObtenerEstadoRallyAsync(rallyId.Value)));
                }
                catch (Exception ex) { return MapError(ex); }
            });
        }

        private static IResult MapError(Exception ex) => ex switch
        {
            ArgumentException =>
                Results.Problem(ex.Message, statusCode: 400),
            InvalidOperationException e when e.Message.Contains("no encontrado") =>
                Results.Problem(ex.Message, statusCode: 404),
            _ =>
                Results.Problem(ex.Message, statusCode: 400)
        };

        private static object? MapDetalle(Fundamento fundamento, DetalleAccionRequest? req)
        {
            if (req == null) return null;
            return fundamento switch
            {
                Fundamento.Saque => new DetalleSaque
                {
                    ZonaSaque = req.ZonaSaque  ?? throw new ArgumentException("ZonaSaque requerida para Saque"),
                    TipoSaque = req.TipoSaque  ?? throw new ArgumentException("TipoSaque requerido para Saque")
                },
                Fundamento.Recepcion => new DetalleRecepcion
                {
                    TipoRecepcion = req.TipoRecepcion ?? throw new ArgumentException("TipoRecepcion requerida para Recepcion")
                },
                Fundamento.Ataque => new DetalleAtaque
                {
                    Lado        = req.Lado        ?? throw new ArgumentException("Lado requerido para Ataque"),
                    TipoAccion  = req.TipoAccion  ?? throw new ArgumentException("TipoAccion requerido para Ataque"),
                    ZonaDestino = req.ZonaDestino ?? throw new ArgumentException("ZonaDestino requerida para Ataque"),
                    EsVarilla   = req.EsVarilla  ?? false,
                    EsEspecial  = req.EsEspecial ?? false
                },
                _ => throw new ArgumentException($"El fundamento {fundamento} no admite detalle")
            };
        }

        private static EstadoRallyResponse MapEstado(EstadoRallyData estado)
        {
            var rally  = estado.Rally;
            var m      = estado.Marcador;
            var nombre = (int id) => estado.NombresJugadores.GetValueOrDefault(id, "");

            SugerenciaPasoResponse? sugerencia = null;
            if (estado.Sugerencia != null)
            {
                var s = estado.Sugerencia;
                sugerencia = new SugerenciaPasoResponse(
                    s.Opciones.Select(o => new OpcionPasoResponse(
                        o.Fundamento,
                        o.JugadorSugeridoId,
                        o.JugadorSugeridoId.HasValue ? nombre(o.JugadorSugeridoId.Value) : null)).ToList(),
                    s.DuplaId, s.Complejo, s.PermiteDe2da, s.JugadorDe2daId, s.EsRejuego);
            }

            return new EstadoRallyResponse(
                rally.Id, rally.NumeroRally,
                rally.MarcadorDupla1, rally.MarcadorDupla2,
                estado.Acciones
                    .Select(a => new AccionResponse(a.Secuencia, a.JugadorId, nombre(a.JugadorId), a.Fundamento, a.Calidad, a.Complejo, a.EsRejuego))
                    .ToList(),
                sugerencia,
                new ResultadoMarcadorResponse(m.SetTerminado, m.PartidoTerminado, m.CambioDeLado, m.DuplaGanadoraSetId),
                rally.DuplaGanadoraId != null,
                rally.DuplaGanadoraId,
                rally.TipoCierre);
        }
    }
}
