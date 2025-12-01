using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;

namespace SandStats.Pages.Jugadores
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Jugador> Jugadores { get; set; } = new List<Jugador>();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public async Task OnGetAsync()
        {
            var q = _context.Jugadores.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var term = Search.Trim();

                q = q.Where(j =>
                    j.Nombre.Contains(term) ||
                    j.Apellido.Contains(term) ||
                    (j.Nombre + " " + j.Apellido).Contains(term));
            }

            Jugadores = await q
                .OrderBy(j => j.Apellido)
                .ThenBy(j => j.Nombre)
                .ToListAsync();
        }
    }
}
