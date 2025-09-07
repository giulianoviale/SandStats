using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using SandStats.Models.SandStats.Models;
using System.Text.RegularExpressions;

namespace SandStats.Pages.Estadisticas
{
    public class EditarRecepcionModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditarRecepcionModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int JugadorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PartidoId { get; set; }

        public Jugador? JugadorSeleccionado { get; set; }
        public string PartidoDescripcion { get; set; } = "";

        // ===== NUEVO: alcance y segmentación =====
        [BindProperty(SupportsGet = true)]
        public ScopeEstadistica Scope { get; set; } = ScopeEstadistica.PartidoCompleto;

        [BindProperty(SupportsGet = true)]
        public int? DesdePunto { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SetNumero { get; set; } // 1, 2 o 3 (tie-break)

        // Mapa: Zona -> Saque -> TipoRecepcion -> Resultado -> Cantidad
        private readonly Dictionary<ZonaSaque, Dictionary<TipoSaque, Dictionary<TipoRecepcion, Dictionary<ResultadoRecepcion, int>>>> _vals = new();

        public int GetValor(ZonaSaque zona, TipoSaque saque, TipoRecepcion tipoRec, ResultadoRecepcion res)
        {
            if (_vals.TryGetValue(zona, out var s) &&
                s.TryGetValue(saque, out var t) &&
                t.TryGetValue(tipoRec, out var r) &&
                r.TryGetValue(res, out var v))
                return v;
            return 0;
        }

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

            // Cargar existentes SOLO del alcance actual
            var query = _context.EstadisticaRecepcion
                .Where(e => e.PartidoId == partidoId
                         && e.JugadorId == jugadorId
                         && e.Scope == Scope);

            if (Scope == ScopeEstadistica.Cierre)
                query = query.Where(e => e.SetNumero == SetNumero);
            else
                query = query.Where(e => e.SetNumero == null);

            var existentes = await query.ToListAsync();

            foreach (var e in existentes)
            {
                if (!_vals.TryGetValue(e.ZonaRecepcion, out var sDict))
                    _vals[e.ZonaRecepcion] = sDict = new();

                if (!sDict.TryGetValue(e.TipoSaque, out var tDict))
                    sDict[e.TipoSaque] = tDict = new();

                if (!tDict.TryGetValue(e.TipoRecepcion, out var rDict))
                    tDict[e.TipoRecepcion] = rDict = new();

                // Cada fila representa 1 recepción
                rDict[e.ResultadoRecepcion] = (rDict.TryGetValue(e.ResultadoRecepcion, out var cur) ? cur : 0) + 1;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var jugadorId = int.Parse(Request.Form["JugadorId"]);
            var partidoId = int.Parse(Request.Form["PartidoId"]);

            // Leer alcance/segmentación del form
            ScopeEstadistica scope = ScopeEstadistica.PartidoCompleto;
            int? desde = null;
            int? set = null;

            if (Request.Form.TryGetValue("Scope", out var scopeVal) && int.TryParse(scopeVal, out int s))
                scope = (ScopeEstadistica)s;

            if (Request.Form.TryGetValue("DesdePunto", out var desdeVal) && int.TryParse(desdeVal, out int d))
                desde = d;

            if (Request.Form.TryGetValue("SetNumero", out var setVal) && int.TryParse(setVal, out int se))
                set = se;

            // Borrar SOLO lo correspondiente al alcance actual
            var toRemove = _context.EstadisticaRecepcion
                .Where(e => e.PartidoId == partidoId
                         && e.JugadorId == jugadorId
                         && e.Scope == scope);

            if (scope == ScopeEstadistica.Cierre)
                toRemove = toRemove.Where(e => e.SetNumero == set);
            else
                toRemove = toRemove.Where(e => e.SetNumero == null);

            var prev = await toRemove.ToListAsync();
            if (prev.Count > 0)
            {
                _context.EstadisticaRecepcion.RemoveRange(prev);
                await _context.SaveChangesAsync();
            }

            // Reinsertar del formulario
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

                        // Etiquetado del alcance
                        Scope = scope,
                        DesdePunto = (scope == ScopeEstadistica.Cierre) ? desde : null,
                        SetNumero = (scope == ScopeEstadistica.Cierre) ? set : null
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Recepción actualizada correctamente.";
            return RedirectToPage("/Estadisticas/Resumen", new { partidoId });
        }
    }
}
