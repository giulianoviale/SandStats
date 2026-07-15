using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models.EnVivo;

namespace SandStats.Pages.EnVivo
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context) => _context = context;

        public List<PartidoEnVivo> Partidos { get; set; } = new();

        public async Task OnGetAsync()
        {
            Partidos = await _context.PartidosEnVivo
                .Include(p => p.Dupla1).ThenInclude(d => d!.Jugador1)
                .Include(p => p.Dupla1).ThenInclude(d => d!.Jugador2)
                .Include(p => p.Dupla2).ThenInclude(d => d!.Jugador1)
                .Include(p => p.Dupla2).ThenInclude(d => d!.Jugador2)
                .Include(p => p.Sets)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();
        }

        public static string NombreDupla(Models.Dupla? dupla)
        {
            if (dupla == null) return "-";
            return !string.IsNullOrWhiteSpace(dupla.Alias)
                ? dupla.Alias
                : dupla.Nombre ?? $"Dupla #{dupla.Id}";
        }
    }
}
