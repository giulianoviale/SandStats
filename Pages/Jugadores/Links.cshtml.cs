using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;

namespace SandStats.Pages.Jugadores
{
    public class LinksModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LinksModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Solo mostramos datos del jugador; no hace falta bindearlos
        public Jugador Jugador { get; set; } = default!;

        [BindProperty]
        public int JugadorId { get; set; }

        [BindProperty]
        public JugadorLinks Links { get; set; } = new();

        public List<SelectListItem> RolesJugador { get; set; } = new();
        public List<SelectListItem> Posiciones { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            JugadorId = id;

            var jugador = await _context.Jugadores
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jugador == null)
                return NotFound();

            Jugador = jugador;

            // Traemos la fila de links (si existe)
            var links = await _context.JugadorLinks
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.JugadorId == id);

            if (links == null)
            {
                Links = new JugadorLinks
                {
                    JugadorId = id
                };
            }
            else
            {
                Links = links;
            }

            CargarCombos(jugador);
            return Page();
        }
        public string NormalizarUrl(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "#";

            var url = raw.Trim();

            // Si ya viene con http/https, la usamos tal cual
            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return url;

            // Si parece un ID de YouTube (11 caracteres sin espacios)
            if (url.Length == 11 && !url.Contains(" "))
                return "https://youtu.be/" + url;

            // Caso genérico: le agregamos https://
            return "https://" + url;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // JugadorId viene del hidden
            var jugador = await _context.Jugadores
                .FirstOrDefaultAsync(j => j.Id == JugadorId);

            if (jugador == null)
                return NotFound();

            Jugador = jugador; // por si volvemos al Page()

            // Buscamos la fila existente de links
            var linksDb = await _context.JugadorLinks
                .FirstOrDefaultAsync(l => l.JugadorId == JugadorId);

            if (linksDb == null)
            {
                linksDb = new JugadorLinks
                {
                    JugadorId = JugadorId
                };
                _context.JugadorLinks.Add(linksDb);
            }

            // Normalizamos textos (trim + null si vacío)
            linksDb.LinkK1 = Sanear(Links.LinkK1);
            linksDb.LinkK2 = Sanear(Links.LinkK2);
            linksDb.LinkSaque = Sanear(Links.LinkSaque);
            linksDb.LinkArmado = Sanear(Links.LinkArmado);
            linksDb.LinkBloqueo = Sanear(Links.LinkBloqueo);
            linksDb.LinkExtra = Sanear(Links.LinkExtra);

            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Links actualizados correctamente.";

            // Volvemos a GET para que lea los valores guardados desde BD
            return RedirectToPage("./Links", new { id = JugadorId });
        }

        private static string? Sanear(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            return raw.Trim();
        }

        private void CargarCombos(Jugador jugador)
        {
            RolesJugador = Enum.GetValues(typeof(RolJugador))
                .Cast<RolJugador>()
                .Select(r => new SelectListItem
                {
                    Value = r.ToString(),
                    Text = r.ToString(),
                    Selected = jugador.RolPrincipal == r
                })
                .ToList();

            Posiciones = Enum.GetValues(typeof(PosicionJugador))
                .Cast<PosicionJugador>()
                .Select(p => new SelectListItem
                {
                    Value = p.ToString(),
                    Text = p.ToString(),
                    Selected = jugador.Posicion == p
                })
                .ToList();
        }
    }
}
