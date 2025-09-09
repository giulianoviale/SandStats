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

        public async Task<IActionResult> OnPost()
        {
            // Validación: duplas distintas
            if (Partido.Dupla1Id == Partido.Dupla2Id)
                ModelState.AddModelError("Partido.Dupla2Id", "No se puede jugar un partido con la misma dupla.");

            // Aseguro estructuras
            Partido.Sets ??= new List<Set>();
            while (Partido.Sets.Count < 3) Partido.Sets.Add(new Set());

            // Completar cada set
            for (int i = 0; i < Partido.Sets.Count; i++)
            {
                var set = Partido.Sets[i];

                // Ignorar el set 3 vacío (si no hubo tie-break)
                if (i == 2 && set.PuntosDupla1 == 0 && set.PuntosDupla2 == 0)
                    continue;

                set.Partido = Partido;
                set.NumeroSet = i + 1;

                // Ganador por puntos
                if (set.PuntosDupla1 != set.PuntosDupla2)
                {
                    set.GanadorDuplaId = (set.PuntosDupla1 > set.PuntosDupla2)
                        ? Partido.Dupla1Id
                        : Partido.Dupla2Id;
                }
            }

            // Si el usuario NO cargó SetsGanados (ambos en 0), los infiero de los sets
            if (Partido.SetsGanadosDupla1 == 0 && Partido.SetsGanadosDupla2 == 0)
            {
                var sg1 = Partido.Sets.Count(s => s.GanadorDuplaId == Partido.Dupla1Id);
                var sg2 = Partido.Sets.Count(s => s.GanadorDuplaId == Partido.Dupla2Id);
                Partido.SetsGanadosDupla1 = sg1;
                Partido.SetsGanadosDupla2 = sg2;
            }
            // 👆 Si el usuario los cargó manualmente, los respetamos (tal como pediste).

            // Revalidar modelo
            ModelState.Clear();
            TryValidateModel(Partido);

            if (!ModelState.IsValid)
            {
                await OnGet(); // recargar combos
                return Page();
            }

            _context.Partidos.Add(Partido);
            await _context.SaveChangesAsync();

            UltimoPartidoId = Partido.Id;               // usado por tu script
            TempData["PartidoIdUltimo"] = Partido.Id;   // (por si usás esta key también)
            TempData["PartidoCreado"] = "1";

            return RedirectToPage("./Index");
        }
    }
}
