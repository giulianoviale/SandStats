using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models.EnVivo;
using SandStats.Services.EnVivo;

namespace SandStats.Pages.EnVivo
{
    public class IniciarSetModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly CargaEnVivoService _svc;

        public IniciarSetModel(ApplicationDbContext context, CargaEnVivoService svc)
        {
            _context = context;
            _svc     = svc;
        }

        [BindProperty(SupportsGet = true)]
        public int PartidoId { get; set; }

        public PartidoEnVivo Partido { get; set; } = default!;
        public int NumeroSet { get; set; }
        public SelectList SacadoresD1 { get; set; } = default!;
        public SelectList SacadoresD2 { get; set; } = default!;
        public string NombreDupla1 { get; set; } = "";
        public string NombreDupla2 { get; set; } = "";

        [BindProperty] public int SacadorInicialD1Id { get; set; }
        [BindProperty] public int SacadorInicialD2Id { get; set; }
        [BindProperty] public int DuplaQueSacaPrimeroId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await CargarPartidoAsync()) return NotFound();
            PreCargarDesdeSetAnterior();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!await CargarPartidoAsync()) return NotFound();

            if (!ModelState.IsValid) return Page();

            var set = await _svc.IniciarSetAsync(
                PartidoId, NumeroSet,
                SacadorInicialD1Id, SacadorInicialD2Id,
                DuplaQueSacaPrimeroId);

            return RedirectToPage("Carga", new { setId = set.Id });
        }

        private async Task<bool> CargarPartidoAsync()
        {
            var partido = await _context.PartidosEnVivo
                .Include(p => p.Dupla1).ThenInclude(d => d!.Jugador1)
                .Include(p => p.Dupla1).ThenInclude(d => d!.Jugador2)
                .Include(p => p.Dupla2).ThenInclude(d => d!.Jugador1)
                .Include(p => p.Dupla2).ThenInclude(d => d!.Jugador2)
                .Include(p => p.Sets)
                .FirstOrDefaultAsync(p => p.Id == PartidoId);

            if (partido == null) return false;

            Partido = partido;
            NumeroSet = partido.Sets.Count + 1;

            var d1 = partido.Dupla1!;
            var d2 = partido.Dupla2!;

            NombreDupla1 = !string.IsNullOrWhiteSpace(d1.Alias) ? d1.Alias
                : $"{d1.Jugador1?.Apellido}/{d1.Jugador2?.Apellido}";
            NombreDupla2 = !string.IsNullOrWhiteSpace(d2.Alias) ? d2.Alias
                : $"{d2.Jugador1?.Apellido}/{d2.Jugador2?.Apellido}";

            SacadoresD1 = new SelectList(new[]
            {
                new { Id = d1.Jugador1!.Id, Nombre = d1.Jugador1.NombreCompleto },
                new { Id = d1.Jugador2!.Id, Nombre = d1.Jugador2.NombreCompleto },
            }, "Id", "Nombre");

            SacadoresD2 = new SelectList(new[]
            {
                new { Id = d2.Jugador1!.Id, Nombre = d2.Jugador1.NombreCompleto },
                new { Id = d2.Jugador2!.Id, Nombre = d2.Jugador2.NombreCompleto },
            }, "Id", "Nombre");

            return true;
        }

        private void PreCargarDesdeSetAnterior()
        {
            var setAnterior = Partido.Sets.OrderByDescending(s => s.NumeroSet).FirstOrDefault();
            if (setAnterior == null) return;

            SacadorInicialD1Id    = setAnterior.SacadorInicialDupla1JugadorId;
            SacadorInicialD2Id    = setAnterior.SacadorInicialDupla2JugadorId;
            DuplaQueSacaPrimeroId = setAnterior.DuplaQueSacaPrimeroId;
        }
    }
}
