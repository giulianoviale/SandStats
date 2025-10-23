using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;

namespace SandStats.Pages.Duplas;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Dupla> Duplas { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }

    private const int PageSize = 10;

    public async Task OnGetAsync(int? pageIndex)
    {
        CurrentPage = pageIndex ?? 1;

        int totalRecords = await _context.Duplas.CountAsync();
        TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

        Duplas = await _context.Duplas
            .Include(d => d.Jugador1)
            .Include(d => d.Jugador2)
            // 🔹 Ordenar alfabéticamente por Jugador1, luego por Jugador2
            .OrderBy(d => d.Jugador1.NombreCompleto)
            .ThenBy(d => d.Jugador2.NombreCompleto)
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
    }

}
