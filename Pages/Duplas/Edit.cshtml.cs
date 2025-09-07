using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;

namespace SandStats.Pages.Duplas
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EditModel(ApplicationDbContext context) => _context = context;

        [BindProperty] public Dupla Dupla { get; set; } = default!;

        public SelectList JugadoresRol4 { get; set; } = default!;
        public SelectList JugadoresRol2 { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Dupla = await _context.Duplas.FindAsync(id);
            if (Dupla is null) return NotFound();

            await CargarListasAsync();
            return Page();
        }

        private async Task CargarListasAsync()
        {
            var rol4 = await _context.Jugadores
                .Where(j => j.RolPrincipal == RolJugador.Rol4)
                .Select(j => new { j.Id, NombreCompleto = j.Apellido + ", " + j.Nombre })
                .OrderBy(j => j.NombreCompleto)
                .ToListAsync();

            var rol2 = await _context.Jugadores
                .Where(j => j.RolPrincipal == RolJugador.Rol2)
                .Select(j => new { j.Id, NombreCompleto = j.Apellido + ", " + j.Nombre })
                .OrderBy(j => j.NombreCompleto)
                .ToListAsync();

            JugadoresRol4 = new SelectList(rol4, "Id", "NombreCompleto", Dupla?.Jugador1Id);
            JugadoresRol2 = new SelectList(rol2, "Id", "NombreCompleto", Dupla?.Jugador2Id);
        }

        private static string ConstruirAlias(Jugador j4, Jugador j2)
        {
            var cmp = StringComparer.OrdinalIgnoreCase;
            bool j4Primero =
                cmp.Compare(j4.Apellido, j2.Apellido) < 0 ||
                (cmp.Compare(j4.Apellido, j2.Apellido) == 0 &&
                 cmp.Compare(j4.Nombre, j2.Nombre) <= 0);

            return j4Primero
                ? $"{j4.Apellido}/{j2.Apellido}"
                : $"{j2.Apellido}/{j4.Apellido}";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validaciones
            if (Dupla.Jugador1Id == Dupla.Jugador2Id)
                ModelState.AddModelError(string.Empty, "Los jugadores deben ser distintos.");

            var j4 = await _context.Jugadores.FindAsync(Dupla.Jugador1Id);
            var j2 = await _context.Jugadores.FindAsync(Dupla.Jugador2Id);

            if (j4 is null || j2 is null)
                ModelState.AddModelError(string.Empty, "Jugadores inválidos.");

            if (j4 != null && j4.RolPrincipal != RolJugador.Rol4)
                ModelState.AddModelError("Dupla.Jugador1Id", "Debe seleccionar un jugador con Rol 4.");
            if (j2 != null && j2.RolPrincipal != RolJugador.Rol2)
                ModelState.AddModelError("Dupla.Jugador2Id", "Debe seleccionar un jugador con Rol 2.");

            // Duplicados (excluyendo la misma dupla que estamos editando)
            bool yaExiste = await _context.Duplas.AsNoTracking().AnyAsync(d =>
                d.Id != Dupla.Id &&
                (
                  (d.Jugador1Id == Dupla.Jugador1Id && d.Jugador2Id == Dupla.Jugador2Id) ||
                  (d.Jugador1Id == Dupla.Jugador2Id && d.Jugador2Id == Dupla.Jugador1Id)
                ));

            if (yaExiste)
                ModelState.AddModelError(string.Empty, "Ya existe una dupla con esos jugadores.");

            if (!ModelState.IsValid)
            {
                await CargarListasAsync();
                return Page();
            }

            // Recalcular Alias SIEMPRE en edición (se te quedaba vacío al cambiar el orden)
            Dupla.Alias = ConstruirAlias(j4!, j2!);

            // Update seguro: traer la entidad original y aplicar cambios
            var original = await _context.Duplas.FirstOrDefaultAsync(d => d.Id == Dupla.Id);
            if (original is null) return NotFound();

            original.Jugador1Id = Dupla.Jugador1Id; // Rol4
            original.Jugador2Id = Dupla.Jugador2Id; // Rol2
            original.Alias = Dupla.Alias;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Duplas.AnyAsync(e => e.Id == Dupla.Id))
                    return NotFound();
                throw;
            }

            TempData["Mensaje"] = "✅ Dupla editada correctamente.";
            return RedirectToPage("./Index");
        }
    }
}
