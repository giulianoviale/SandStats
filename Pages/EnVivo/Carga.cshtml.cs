using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;

namespace SandStats.Pages.EnVivo
{
    public class CargaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CargaModel(ApplicationDbContext context) => _context = context;

        [BindProperty(SupportsGet = true)]
        public int SetId { get; set; }

        public string NombreDupla1 { get; set; } = "";
        public string NombreDupla2 { get; set; } = "";
        public int NumeroSet { get; set; }
        public int PartidoId { get; set; }
        public int Dupla1Id { get; set; }
        public int Dupla2Id { get; set; }
        public string JugadoresJson { get; set; } = "{}";

        public async Task<IActionResult> OnGetAsync()
        {
            var set = await _context.SetsEnVivo
                .Include(s => s.PartidoEnVivo)
                    .ThenInclude(p => p!.Dupla1).ThenInclude(d => d!.Jugador1)
                .Include(s => s.PartidoEnVivo)
                    .ThenInclude(p => p!.Dupla1).ThenInclude(d => d!.Jugador2)
                .Include(s => s.PartidoEnVivo)
                    .ThenInclude(p => p!.Dupla2).ThenInclude(d => d!.Jugador1)
                .Include(s => s.PartidoEnVivo)
                    .ThenInclude(p => p!.Dupla2).ThenInclude(d => d!.Jugador2)
                .FirstOrDefaultAsync(s => s.Id == SetId);

            if (set == null) return NotFound();

            var partido = set.PartidoEnVivo!;
            var d1 = partido.Dupla1!;
            var d2 = partido.Dupla2!;

            NumeroSet  = set.NumeroSet;
            PartidoId  = partido.Id;
            Dupla1Id   = d1.Id;
            Dupla2Id   = d2.Id;

            NombreDupla1 = !string.IsNullOrWhiteSpace(d1.Alias) ? d1.Alias
                : $"{d1.Jugador1?.Apellido}/{d1.Jugador2?.Apellido}";
            NombreDupla2 = !string.IsNullOrWhiteSpace(d2.Alias) ? d2.Alias
                : $"{d2.Jugador1?.Apellido}/{d2.Jugador2?.Apellido}";

            JugadoresJson = JsonSerializer.Serialize(new Dictionary<int, object>
            {
                [d1.Id] = new[]
                {
                    new { id = d1.Jugador1!.Id, nombre = d1.Jugador1.NombreCompleto },
                    new { id = d1.Jugador2!.Id, nombre = d1.Jugador2.NombreCompleto }
                },
                [d2.Id] = new[]
                {
                    new { id = d2.Jugador1!.Id, nombre = d2.Jugador1.NombreCompleto },
                    new { id = d2.Jugador2!.Id, nombre = d2.Jugador2.NombreCompleto }
                }
            });

            return Page();
        }
    }
}
