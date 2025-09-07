using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;

namespace SandStats.Pages.Jugadores;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Jugador> Jugadores { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public int CurrentPage => PageIndex;

    public async Task<IActionResult> OnGetAsync()
    {
        var total = await _context.Jugadores.CountAsync();
        TotalPages = (int)Math.Ceiling(total / (double)PageSize);

        Jugadores = await _context.Jugadores
            .OrderBy(j => j.Apellido)
            .ThenBy(j => j.Nombre)
            .Skip((PageIndex - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return Page();
    }
}
