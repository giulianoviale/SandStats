using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using SandStats.Data;
using SandStats.Models;
using SandStats.Models.EnVivo;
using Xunit;

namespace SandStats.Tests
{
    public class EnVivoEndpointsTests : IClassFixture<EnVivoWebFactory>
    {
        private readonly HttpClient _client;
        private readonly EnVivoWebFactory _factory;

        public EnVivoEndpointsTests(EnVivoWebFactory factory)
        {
            _factory = factory;
            _client  = factory.CreateClient();
        }

        // Seed: 4 jugadores + 2 duplas nuevos por test (IDs auto-incrementados, sin interferencia)
        private async Task<(int d1Id, int d2Id, int j1Id, int j3Id)> SeedDuplasAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var j1 = new Jugador { Nombre = "A", Apellido = "D1", Posicion = PosicionJugador.Bloqueador, RolPrincipal = RolJugador.Rol4 };
            var j2 = new Jugador { Nombre = "B", Apellido = "D1", Posicion = PosicionJugador.Defensor,   RolPrincipal = RolJugador.Rol2 };
            var j3 = new Jugador { Nombre = "C", Apellido = "D2", Posicion = PosicionJugador.Bloqueador, RolPrincipal = RolJugador.Rol4 };
            var j4 = new Jugador { Nombre = "D", Apellido = "D2", Posicion = PosicionJugador.Defensor,   RolPrincipal = RolJugador.Rol2 };
            db.Jugadores.AddRange(j1, j2, j3, j4);
            await db.SaveChangesAsync();

            var d1 = new Dupla { Jugador1Id = j1.Id, Jugador2Id = j2.Id };
            var d2 = new Dupla { Jugador1Id = j3.Id, Jugador2Id = j4.Id };
            db.Duplas.AddRange(d1, d2);
            await db.SaveChangesAsync();

            return (d1.Id, d2.Id, j1.Id, j3.Id);
        }

        private async Task<(int partidoId, int setId, int rallyId)> CrearPartidoSetRallyAsync(
            int d1Id, int d2Id, int j1Id, int j3Id)
        {
            var rP = await _client.PostAsJsonAsync("/api/envivo/partidos",
                new { dupla1Id = d1Id, dupla2Id = d2Id, torneo = "T", fecha = DateTime.UtcNow });
            rP.EnsureSuccessStatusCode();
            var pj = JsonDocument.Parse(await rP.Content.ReadAsStringAsync()).RootElement;
            int pId = pj.GetProperty("id").GetInt32();

            var rS = await _client.PostAsJsonAsync($"/api/envivo/partidos/{pId}/sets",
                new { numeroSet = 1, sacadorInicialD1Id = j1Id, sacadorInicialD2Id = j3Id, duplaQueSacaPrimeroId = d1Id });
            rS.EnsureSuccessStatusCode();
            var sj = JsonDocument.Parse(await rS.Content.ReadAsStringAsync()).RootElement;
            int sId = sj.GetProperty("id").GetInt32();

            var rR = await _client.PostAsJsonAsync($"/api/envivo/sets/{sId}/rallies", new { });
            rR.EnsureSuccessStatusCode();
            var rj = JsonDocument.Parse(await rR.Content.ReadAsStringAsync()).RootElement;
            int rallyId = rj.GetProperty("id").GetInt32();

            return (pId, sId, rallyId);
        }

        // ── Test 1: flujo completo 4 acciones → cerrado, marcador 0-1 ─────────

        [Fact]
        public async Task FlujoCopleto_4Acciones_EstadoFinalCerradoYMarcadorCorrecto()
        {
            var (d1Id, d2Id, j1Id, j3Id) = await SeedDuplasAsync();
            var (_, _, rallyId) = await CrearPartidoSetRallyAsync(d1Id, d2Id, j1Id, j3Id);

            // Saque(null, J1)
            await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/acciones",
                new { fundamento = "Saque", calidad = (string?)null, jugadorId = j1Id, esDe2da = false, detalle = (object?)null });

            // Recepcion(Positivo, J3) → deriva Saque→Negativo
            await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/acciones",
                new { fundamento = "Recepcion", calidad = "Positivo", jugadorId = j3Id, esDe2da = false, detalle = (object?)null });

            // Ataque(null, J3)
            await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/acciones",
                new { fundamento = "Ataque", calidad = (string?)null, jugadorId = j3Id, esDe2da = false, detalle = (object?)null });

            // Bloqueo(DobleNegativo, J1) → deriva Ataque→DoblePositivo, cierra D2
            var r4 = await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/acciones",
                new { fundamento = "Bloqueo", calidad = "DobleNegativo", jugadorId = j1Id, esDe2da = false, detalle = (object?)null });
            r4.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await r4.Content.ReadAsStringAsync());
            var root = doc.RootElement;

            Assert.True(root.GetProperty("cerrado").GetBoolean());
            Assert.Equal(0, root.GetProperty("marcadorDupla1").GetInt32());
            Assert.Equal(1, root.GetProperty("marcadorDupla2").GetInt32());
        }

        // ── Test 2: registrar acción en rally cerrado → 400 ──────────────────

        [Fact]
        public async Task RegistrarAccion_RallyCerrado_Retorna400()
        {
            var (d1Id, d2Id, j1Id, j3Id) = await SeedDuplasAsync();
            var (_, _, rallyId) = await CrearPartidoSetRallyAsync(d1Id, d2Id, j1Id, j3Id);

            // Cerrar el rally
            var rc = await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/cierre-directo",
                new { tipo = "Ace", duplaGanadoraId = (int?)null });
            rc.EnsureSuccessStatusCode();

            // Intentar registrar acción en rally ya cerrado
            var r = await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/acciones",
                new { fundamento = "Saque", calidad = (string?)null, jugadorId = j1Id, esDe2da = false, detalle = (object?)null });

            Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
        }

        // ── Test 3: deshacer en rally abierto sin acciones → 400 ─────────────

        [Fact]
        public async Task Deshacer_RallyAbiertoVacio_Retorna400()
        {
            var (d1Id, d2Id, j1Id, j3Id) = await SeedDuplasAsync();
            var (_, _, rallyId) = await CrearPartidoSetRallyAsync(d1Id, d2Id, j1Id, j3Id);

            var r = await _client.PostAsJsonAsync($"/api/envivo/rallies/{rallyId}/deshacer", new { });
            Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
        }

        // ── Test 4: rally inexistente → 404 ──────────────────────────────────

        [Fact]
        public async Task RallyInexistente_Retorna404()
        {
            var r = await _client.PostAsJsonAsync("/api/envivo/rallies/99999/acciones",
                new { fundamento = "Saque", calidad = (string?)null, jugadorId = 1, esDe2da = false, detalle = (object?)null });
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
        }

        // ── Test 5: GET estado con rally vacío → sugerencia es Saque ─────────

        [Fact]
        public async Task GetEstado_RallyVacio_SugerenciaEsSaque()
        {
            var (d1Id, d2Id, j1Id, j3Id) = await SeedDuplasAsync();
            var (_, setId, _) = await CrearPartidoSetRallyAsync(d1Id, d2Id, j1Id, j3Id);

            var r = await _client.GetAsync($"/api/envivo/sets/{setId}/estado");
            r.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await r.Content.ReadAsStringAsync());
            var opciones = doc.RootElement
                .GetProperty("sugerencia")
                .GetProperty("opciones");

            Assert.Equal("Saque", opciones[0].GetProperty("fundamento").GetString());
        }
    }
}
