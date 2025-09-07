using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using SandStats.Models.SandStats.Models;
using System.Text.RegularExpressions;

namespace SandStats.Pages.Estadisticas
{
    public class CargarRecepcionModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CargarRecepcionModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int JugadorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PartidoId { get; set; }

        public Jugador? JugadorSeleccionado { get; set; }
        public string PartidoDescripcion { get; set; } = "";

        // ===== NUEVO: alcance y segmentación (como en Ataque) =====
        [BindProperty(SupportsGet = true)]
        public ScopeEstadistica Scope { get; set; } = ScopeEstadistica.PartidoCompleto;

        [BindProperty(SupportsGet = true)]
        public int? DesdePunto { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SetNumero { get; set; } // 1, 2 o 3 (tie-break)

        public async Task<IActionResult> OnGetAsync(int jugadorId, int partidoId)
        {
            JugadorId = jugadorId;
            PartidoId = partidoId;

            var partido = await _context.Partidos
                .Include(p => p.Dupla1).Include(p => p.Dupla2)
                .FirstOrDefaultAsync(p => p.Id == partidoId);

            if (partido == null) return NotFound();

            PartidoDescripcion = partido.Descripcion ?? $"Partido #{partidoId}";
            JugadorSeleccionado = await _context.Jugadores.FindAsync(jugadorId);
            if (JugadorSeleccionado == null) return NotFound();

            // Defaults inteligentes para "Cierre"
            if (Scope == ScopeEstadistica.Cierre)
            {
                if (!DesdePunto.HasValue)
                    DesdePunto = (SetNumero == 3) ? 11 : 16; // sets a 21 -> 16; TB a 15 -> 11
            }
            else
            {
                DesdePunto = null;
                SetNumero = null;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var jugadorId = int.Parse(Request.Form["JugadorId"]);
            var partidoId = int.Parse(Request.Form["PartidoId"]);

            // ===== leer alcance/segmentación del form =====
            ScopeEstadistica scope = ScopeEstadistica.PartidoCompleto;
            int? desde = null;
            int? set = null;

            if (Request.Form.TryGetValue("Scope", out var scopeVal) && int.TryParse(scopeVal, out int s))
                scope = (ScopeEstadistica)s;

            if (Request.Form.TryGetValue("DesdePunto", out var desdeVal) && int.TryParse(desdeVal, out int d))
                desde = d;

            if (Request.Form.TryGetValue("SetNumero", out var setVal) && int.TryParse(setVal, out int se))
                set = se;

            foreach (var key in Request.Form.Keys)
            {
                if (!key.StartsWith("Recepciones[")) continue;

                // Recepciones[Zona][TipoSaque][TipoRecepcion][Resultado]
                var match = Regex.Match(key, @"^Recepciones\[(.+?)\]\[(.+?)\]\[(.+?)\]\[(.+?)\]$");
                if (!match.Success) continue;

                var zonaStr = match.Groups[1].Value;
                var saqueStr = match.Groups[2].Value;
                var tipoRecStr = match.Groups[3].Value;
                var resStr = match.Groups[4].Value;

                if (!int.TryParse(Request.Form[key], out var cantidad) || cantidad <= 0)
                    continue;

                var zona = Enum.Parse<ZonaSaque>(zonaStr, true);
                var tipoSaque = Enum.Parse<TipoSaque>(saqueStr, true);
                var tipoRec = Enum.Parse<TipoRecepcion>(tipoRecStr, true);
                var resultado = Enum.Parse<ResultadoRecepcion>(resStr, true);

                // Tu modelo no tiene "Cantidad": insertamos N filas
                for (int i = 0; i < cantidad; i++)
                {
                    _context.EstadisticaRecepcion.Add(new EstadisticaRecepcion
                    {
                        JugadorId = jugadorId,
                        PartidoId = partidoId,
                        ZonaRecepcion = zona,
                        TipoSaque = tipoSaque,
                        TipoRecepcion = tipoRec,
                        ResultadoRecepcion = resultado,

                        // NUEVO: etiquetado del alcance
                        Scope = scope,
                        DesdePunto = (scope == ScopeEstadistica.Cierre) ? desde : null,
                        SetNumero = (scope == ScopeEstadistica.Cierre) ? set : null
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Recepción guardada correctamente.";

            return RedirectToPage("/Estadisticas/Resumen", new { partidoId });
        }
    }
}
