using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Services.EnVivo;

namespace SandStats.Pages.EnVivo
{
    public class NuevoPartidoModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly CargaEnVivoService _svc;

        public NuevoPartidoModel(ApplicationDbContext context, CargaEnVivoService svc)
        {
            _context = context;
            _svc     = svc;
        }

        public SelectList Duplas { get; set; } = default!;

        [BindProperty] public int Dupla1Id { get; set; }
        [BindProperty] public int Dupla2Id { get; set; }
        [BindProperty] public string Torneo { get; set; } = "";
        [BindProperty] public DateTime Fecha { get; set; } = DateTime.Today;

        public async Task OnGetAsync() => await CargarDuplasAsync();

        public async Task<IActionResult> OnPostAsync()
        {
            if (Dupla1Id == Dupla2Id)
                ModelState.AddModelError(nameof(Dupla2Id), "Las dos duplas deben ser distintas.");

            if (!ModelState.IsValid)
            {
                await CargarDuplasAsync();
                return Page();
            }

            var partido = await _svc.CrearPartidoAsync(Dupla1Id, Dupla2Id, Torneo, Fecha);
            return RedirectToPage("IniciarSet", new { partidoId = partido.Id });
        }

        private async Task CargarDuplasAsync()
        {
            var duplas = await _context.Duplas
                .AsNoTracking()
                .Include(d => d.Jugador1)
                .Include(d => d.Jugador2)
                .OrderBy(d => d.Alias)
                .Select(d => new
                {
                    d.Id,
                    Nombre = !string.IsNullOrWhiteSpace(d.Alias)
                        ? d.Alias
                        : (d.Jugador1 != null && d.Jugador2 != null
                            ? d.Jugador1.Apellido + "/" + d.Jugador2.Apellido
                            : $"Dupla #{d.Id}")
                })
                .ToListAsync();

            Duplas = new SelectList(duplas, "Id", "Nombre");
        }
    }
}
