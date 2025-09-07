using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SandStats.Data;
using SandStats.Models;

namespace SandStats.Pages.Jugadores;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public CreateModel(ApplicationDbContext context) => _context = context;

    [BindProperty] public Jugador Jugador { get; set; } = new();

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        _context.Jugadores.Add(Jugador);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Jugador creado correctamente.";
        return RedirectToPage("./Index");
    }
}
