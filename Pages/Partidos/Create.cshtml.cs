using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SandStats.Pages.Partidos
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context) => _context = context;

        [TempData] public int? UltimoPartidoId { get; set; }

        [BindProperty] public Partido Partido { get; set; } = new();   // 👈 inicializado

        public SelectList Duplas { get; set; } = default!;

        public async Task OnGet()
        {
            // Iniciales seguros para la vista
            if (Partido.Fecha == default) Partido.Fecha = DateTime.Today;
            if (Partido.Sets == null || Partido.Sets.Count == 0)
            {
                // Precreo 3 slots para bindear Set 1, 2 y 3 (tie-break opcional)
                Partido.Sets = new List<Set> { new(), new(), new() };
            }

            // Duplas con Include y fallback de alias (por si en el futuro hay nulls)
            var duplas = await _context.Duplas
                .AsNoTracking()
                .Include(d => d.Jugador1)
                .Include(d => d.Jugador2)
                .OrderBy(d => d.Alias)
                .Select(d => new
                {
                    d.Id,
                    Nombre = string.IsNullOrWhiteSpace(d.Alias)
                        ? ((d.Jugador1 != null && d.Jugador2 != null)
                            ? (d.Jugador1.Apellido + "/" + d.Jugador2.Apellido)
                            : $"Dupla #{d.Id}")
                        : d.Alias
                })
                .ToListAsync();

            Duplas = new SelectList(duplas, "Id", "Nombre");
        }

        public IActionResult OnPost()
        {
            void RepoblarCombos() =>
                Duplas = new SelectList(_context.Duplas.AsNoTracking(), "Id", "Alias");

            // 0) Validación básica
            if (Partido.Dupla1Id == Partido.Dupla2Id)
                ModelState.AddModelError("Partido.Dupla2Id", "No se puede jugar un partido con la misma dupla.");

            Partido.Sets ??= new List<Set>();
            // ✅ Normalizar la fecha que viene del <input type="date">
            // (viene como Kind=Unspecified). Marcamos UTC y, si querés, solo la parte de fecha.
            Partido.Fecha = DateTime.SpecifyKind(Partido.Fecha.Date, DateTimeKind.Utc);
            // 1) Quedarnos solo con sets que tienen algún puntaje (evita el "" -> 0 del set 3 oculto)
            var setsCargados = Partido.Sets
                .Where(s => (s?.PuntosDupla1 ?? 0) > 0 || (s?.PuntosDupla2 ?? 0) > 0)
                .ToList();

            if (setsCargados.Count == 0)
                ModelState.AddModelError(string.Empty, "Debe cargar al menos un set con puntajes.");

            // 2) Armar/validar cada set y calcular ganadores
            int ganados1 = 0, ganados2 = 0;
            for (int i = 0; i < setsCargados.Count; i++)
            {
                var set = setsCargados[i];

                set.Partido = Partido;          // EF setea FK
                set.NumeroSet = i + 1;

                var p1 = set.PuntosDupla1;
                var p2 = set.PuntosDupla2;

                if (p1 == p2)
                {
                    ModelState.AddModelError(string.Empty, $"El Set {set.NumeroSet} no tiene ganador (puntajes iguales).");
                    continue;
                }

                set.GanadorDuplaId = (p1 > p2) ? Partido.Dupla1Id : Partido.Dupla2Id;

                if (set.GanadorDuplaId == Partido.Dupla1Id) ganados1++; else ganados2++;
            }

            // 3) Reemplazar colección por la normalizada
            Partido.Sets = setsCargados;

            if (!ModelState.IsValid)
            {
                RepoblarCombos();
                return Page();
            }

            // 4) Mantener tu lógica de carga manual de SetsGanados.
            //    Si vienen en 0, los completamos con lo calculado para no romper reportes.
            if (Partido.SetsGanadosDupla1 == 0 && Partido.SetsGanadosDupla2 == 0)
            {
                Partido.SetsGanadosDupla1 = ganados1;
                Partido.SetsGanadosDupla2 = ganados2;
            }
            else
            {
                // (Opcional) coherencia: la suma debería coincidir con la cantidad de sets cargados
                var suma = Partido.SetsGanadosDupla1 + Partido.SetsGanadosDupla2;
                if (suma != setsCargados.Count)
                {
                    ModelState.AddModelError(string.Empty,
                        $"La suma de sets ganados ({suma}) no coincide con los sets cargados ({setsCargados.Count}).");
                    RepoblarCombos();
                    return Page();
                }
            }
            try
            {
                _context.Partidos.Add(Partido);
                _context.SaveChanges();

                UltimoPartidoId = Partido.Id; // por si lo usás en la UI
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                ModelState.AddModelError(string.Empty, $"No se pudo guardar el partido. {msg}");
                RepoblarCombos();
                return Page();
            }
        }



    }
}
