using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;

namespace SandStats.Pages.Partidos;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Partido> Partidos { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;

    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public int CurrentPage => PageIndex;

    public async Task<IActionResult> OnGetAsync()
    {
        var total = await _context.Partidos.CountAsync();
        TotalPages = (int)Math.Ceiling(total / (double)PageSize);

        Partidos = await _context.Partidos
            .Include(p => p.Dupla1)
            .Include(p => p.Dupla2)
            .OrderByDescending(p => p.CreatedOn)  // 🔹 muestra los últimos ingresados primero
            .ThenByDescending(p => p.Fecha)      // 🔹 luego por fecha del partido
            .Skip((PageIndex - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        return Page();
    }
}
