using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using System.Text.RegularExpressions;

namespace SandStats.Pages.Estadisticas
{
    public class EditarK2Model : PageModel
    {
        private readonly ApplicationDbContext _db;
        public EditarK2Model(ApplicationDbContext db) => _db = db;
        [BindProperty(SupportsGet = true)]
        public ScopeEstadistica Scope { get; set; } = ScopeEstadistica.PartidoCompleto;
        [BindProperty(SupportsGet = true)] public int JugadorId { get; set; }
        [BindProperty(SupportsGet = true)] public int PartidoId { get; set; }

        public Jugador? Jugador { get; private set; }
        public string PartidoDescripcion { get; private set; } = "";

        // cache en memoria para rellenar inputs
        private List<EstadisticaK2> _datos = new();

        // Extras
        public int SetsJugados { get; set; }
        public int ErroresVarios { get; set; }
        public int Agregados { get; set; }

        public int GetValor(FuenteK2 fuente, ResultadoK2 res) =>
            _datos.Where(x => x.Fuente == fuente && x.Resultado == res).Sum(x => x.Cantidad);

        public async Task<IActionResult> OnGetAsync(int jugadorId, int partidoId)
        {
            
            

            JugadorId = jugadorId; PartidoId = partidoId;

            var partido = await _db.Partidos
                .Include(p => p.Dupla1).Include(p => p.Dupla2)
                .FirstOrDefaultAsync(p => p.Id == partidoId);
            if (partido is null) return NotFound();

            PartidoDescripcion = partido.Descripcion ?? $"Partido #{partidoId}";
            Jugador = await _db.Jugadores.FindAsync(jugadorId);
            var sets = partido?.SetsJugados ?? 0;
            _datos = await _db.EstadisticaK2
                .Where(e => e.PartidoId == partidoId && e.JugadorId == jugadorId)
                .ToListAsync();

            // Extras (si existen en alguna fila)
            var any = _datos.FirstOrDefault(x => x.SetsJugados.HasValue || x.Agregados.HasValue || x.ErroresVarios.HasValue);
            if (any != null)
            {
                SetsJugados = any.SetsJugados ?? 0;
                ErroresVarios = any.ErroresVarios ?? 0;
                Agregados = any.Agregados ?? 0;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // --- 1) Borramos lo previo (para este jugador/partido/scope) ---
            await _db.EstadisticaK2
                .Where(e => e.PartidoId == PartidoId
                         && e.JugadorId == JugadorId
                         && e.Scope == Scope)
                .ExecuteDeleteAsync();

            // --- 2) Leemos "extras" del formulario (opcionales) ---
            int? setsJugados = TryGetInt(Request.Form["SetsJugados"]);
            int? erroresVarios = TryGetInt(Request.Form["ErroresVarios"]);
            int? agregados = TryGetInt(Request.Form["Agregados"]);
            int? desdePunto = TryGetInt(Request.Form["DesdePunto"]); // por compatibilidad si lo mostrás
            int? setNumero = TryGetInt(Request.Form["SetNumero"]);

            // --- 3) Parseamos los casilleros: K2[Fuente][Resultado] = cantidad ---
            //     Espera nombres como:
            //     K2[PuntosJugados][DoblePositivo], K2[SaqueFlotado][Positivo], ...
            //     K2[BloqueoAtqA1][Negativo], etc.
            var rx = new Regex(@"^K2\[(.+?)\]\[(.+?)\]$", RegexOptions.IgnoreCase);

            var aInsertar = new List<EstadisticaK2>();

            foreach (var key in Request.Form.Keys)
            {
                var m = rx.Match(key);
                if (!m.Success) continue;

                if (!int.TryParse(Request.Form[key], out var cant) || cant <= 0)
                    continue;

                // Fuente y Resultado desde los nombres del input
                if (!Enum.TryParse<FuenteK2>(m.Groups[1].Value, ignoreCase: true, out var fuente))
                    continue;

                if (!Enum.TryParse<ResultadoK2>(m.Groups[2].Value, ignoreCase: true, out var resultado))
                    continue;

                aInsertar.Add(new EstadisticaK2
                {
                    PartidoId = PartidoId,
                    JugadorId = JugadorId,
                    Fuente = fuente,
                    Resultado = resultado,
                    Cantidad = cant,
                    FechaCarga = DateTime.Now,

                    // Scope & extras (mismo valor en todas las filas)
                    Scope = Scope,
                    DesdePunto = Scope == ScopeEstadistica.Cierre ? desdePunto : null,
                    SetNumero = Scope == ScopeEstadistica.Cierre ? setNumero : null,

                    SetsJugados = setsJugados,
                    ErroresVarios = erroresVarios,
                    Agregados = agregados
                });
            }

            if (aInsertar.Count > 0)
            {
                _db.EstadisticaK2.AddRange(aInsertar);
                await _db.SaveChangesAsync();
            }

            TempData["Mensaje"] = "K2 guardado correctamente.";

            // --- 4) Redirección al reporte ---
            // A) Si querés volver al Resumen del partido:
            return RedirectToPage("/Estadisticas/Resumen", new { partidoId = PartidoId });

            // B) Si preferís ReporteDupla, reemplazá por esto (necesitás DuplaId):
            // return RedirectToPage("/Estadisticas/ReporteDupla", new { duplaId = <DuplaId>, partidoId = PartidoId });
        }// Helper seguro para leer enteros opcionales
        private static int? TryGetInt(string s)
            => int.TryParse(s, out var v) ? v : (int?)null;
    }

}
