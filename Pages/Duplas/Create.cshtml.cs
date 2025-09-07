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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public CreateModel(ApplicationDbContext context) => _context = context;

        [BindProperty] public Dupla Dupla { get; set; } = new();

        // Dropdowns separados por rol
        public SelectList JugadoresRol4 { get; set; } = default!;
        public SelectList JugadoresRol2 { get; set; } = default!;

        public void OnGet() => CargarListas();

        private void CargarListas()
        {
            var rol4 = _context.Jugadores
                .Where(j => j.RolPrincipal == RolJugador.Rol4)
                .Select(j => new { j.Id, NombreCompleto = j.Apellido + ", " + j.Nombre })
                .OrderBy(j => j.NombreCompleto)
                .ToList();

            var rol2 = _context.Jugadores
                .Where(j => j.RolPrincipal == RolJugador.Rol2)
                .Select(j => new { j.Id, NombreCompleto = j.Apellido + ", " + j.Nombre })
                .OrderBy(j => j.NombreCompleto)
                .ToList();

            JugadoresRol4 = new SelectList(rol4, "Id", "NombreCompleto");
            JugadoresRol2 = new SelectList(rol2, "Id", "NombreCompleto");
        }

        private static string ConstruirAlias(Jugador j4, Jugador j2)
        {
            // Alias ordenado alfabéticamente por Apellido/Nombre, como hacías
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
            // Validación básica: distintos
            if (Dupla.Jugador1Id == Dupla.Jugador2Id)
            {
                const string msg = "No se puede seleccionar el mismo jugador en ambas posiciones.";
                ModelState.AddModelError(string.Empty, msg);
            }

            // Cargar jugadores seleccionados
            var j4 = await _context.Jugadores.FindAsync(Dupla.Jugador1Id);
            var j2 = await _context.Jugadores.FindAsync(Dupla.Jugador2Id);

            if (j4 is null || j2 is null)
                ModelState.AddModelError(string.Empty, "Jugadores inválidos.");

            // Validar roles: Jugador1Id debe ser Rol4, Jugador2Id debe ser Rol2
            if (j4 != null && j4.RolPrincipal != RolJugador.Rol4)
                ModelState.AddModelError("Dupla.Jugador1Id", "Debe seleccionar un jugador con Rol 4.");
            if (j2 != null && j2.RolPrincipal != RolJugador.Rol2)
                ModelState.AddModelError("Dupla.Jugador2Id", "Debe seleccionar un jugador con Rol 2.");

            // Duplicado: misma dupla (Rol4, Rol2). También chequeo inverso por seguridad por si queda legacy
            bool yaExiste = await _context.Duplas.AsNoTracking().AnyAsync(d =>
                (d.Jugador1Id == Dupla.Jugador1Id && d.Jugador2Id == Dupla.Jugador2Id) ||
                (d.Jugador1Id == Dupla.Jugador2Id && d.Jugador2Id == Dupla.Jugador1Id));

            if (yaExiste)
            {
                const string msg = "Ya existe una dupla con esos jugadores.";
                ModelState.AddModelError(string.Empty, msg);
            }

            if (!ModelState.IsValid)
            {
                CargarListas();
                return Page();
            }

            // Armar alias
            Dupla.Alias = ConstruirAlias(j4!, j2!);

            _context.Duplas.Add(Dupla);

            try
            {
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "✅ Dupla creada correctamente.";
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                // Si hay UNIQUE en DB, capturo
                var msg = ex.InnerException?.Message ?? ex.Message;
                if (msg.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ||
                    msg.Contains("constraint", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "Ya existe una dupla con esos jugadores.");
                    CargarListas();
                    return Page();
                }
                throw;
            }
        }
    }
}
