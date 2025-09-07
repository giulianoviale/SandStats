using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;

public class EditarJugadorModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public EditarJugadorModel(ApplicationDbContext context) => _context = context;

    [BindProperty]
    public Jugador Jugador { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Jugador = await _context.Jugadores.FirstOrDefaultAsync(j => j.Id == id)
                  ?? throw new InvalidOperationException("Jugador no encontrado");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var dbJugador = await _context.Jugadores.FirstOrDefaultAsync(j => j.Id == Jugador.Id);
        if (dbJugador is null) return NotFound();

        // Actualiza SOLO los campos permitidos (incluye Posicion)
        if (await TryUpdateModelAsync(dbJugador, "Jugador",
            j => j.Nombre, j => j.Apellido, j => j.RolPrincipal, j => j.Posicion))
        {
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Jugador actualizado.";
            return RedirectToPage("./Index");
        }

        // Si algo falló, vuelve con validación
        return Page();
    }
}
