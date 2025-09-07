using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;

namespace SandStats.Pages.Partidos;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public DeleteModel(ApplicationDbContext context) => _context = context;

    public Partido Partido { get; set; } = default!;
    [TempData] public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var partido = await _context.Partidos
            .Include(p => p.Dupla1)
            .Include(p => p.Dupla2)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (partido is null) return NotFound();
        Partido = partido;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var partido = await _context.Partidos.FindAsync(id);
        if (partido is null) return NotFound();

        try
        {
            // Si NO configuraste Cascade en EF, limpiamos manualmente las stats.
            var atq = _context.EstadisticaAtaque.Where(e => e.PartidoId == id);
            var rec = _context.EstadisticaRecepcion.Where(e => e.PartidoId == id);
            var k2 = _context.EstadisticaK2.Where(e => e.PartidoId == id);

            _context.EstadisticaAtaque.RemoveRange(atq);
            _context.EstadisticaRecepcion.RemoveRange(rec);
            _context.EstadisticaK2.RemoveRange(k2);

            _context.Partidos.Remove(partido);

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Partido eliminado.";
            return RedirectToPage("./Index");
        }
        catch (DbUpdateException)
        {
            ErrorMessage = "No se pudo eliminar el partido por restricciones de base de datos.";
            // Re-cargar para re-render si es necesario
            Partido = await _context.Partidos
                .Include(p => p.Dupla1)
                .Include(p => p.Dupla2)
                .FirstOrDefaultAsync(p => p.Id == id) ?? new Partido();
            return Page();
        }
    }
}
