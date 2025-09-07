using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;

namespace SandStats.Pages.Duplas;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public DeleteModel(ApplicationDbContext context) => _context = context;

    public Dupla Dupla { get; set; } = default!;
    public int PartidosAsociados { get; set; }
    [TempData] public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var dupla = await _context.Duplas
            .Include(d => d.Jugador1)
            .Include(d => d.Jugador2)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dupla is null) return NotFound();
        Dupla = dupla;

        PartidosAsociados = await _context.Partidos
            .CountAsync(p => p.Dupla1Id == id || p.Dupla2Id == id);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var dupla = await _context.Duplas
            .Include(d => d.Jugador1)
            .Include(d => d.Jugador2)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dupla is null) return NotFound();

        // Bloquear si tiene partidos
        var usada = await _context.Partidos.AnyAsync(p => p.Dupla1Id == id || p.Dupla2Id == id);
        if (usada)
        {
            ErrorMessage = "No se puede eliminar: la dupla tiene partidos asociados.";
            Dupla = dupla;
            PartidosAsociados = await _context.Partidos
                .CountAsync(p => p.Dupla1Id == id || p.Dupla2Id == id);
            return Page();
        }

        // Desasignar jugadores que apuntan a la dupla (por si hay FK Jugador.DuplaId)
        var jugadoresConDupla = await _context.Jugadores
            .Where(j => j.DuplaId == id)
            .ToListAsync();
        foreach (var j in jugadoresConDupla)
            j.DuplaId = null;

        try
        {
            _context.Duplas.Remove(dupla);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Dupla eliminada.";
            return RedirectToPage("./Index");
        }
        catch (DbUpdateException)
        {
            ErrorMessage = "No se pudo eliminar la dupla por restricciones de base de datos.";
            Dupla = dupla;
            return Page();
        }
    }
}
