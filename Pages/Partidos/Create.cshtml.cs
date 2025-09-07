using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SandStats.Data;
using SandStats.Models;
using System.Collections.Generic;

namespace SandStats.Pages.Partidos
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [TempData]
        public int? UltimoPartidoId { get; set; }

        [BindProperty]
        public Partido Partido { get; set; }

        public SelectList Duplas { get; set; }

        public void OnGet()
        {
            Duplas = new SelectList(_context.Duplas, "Id", "Alias");
        }

        public IActionResult OnPost()
        {
            // Validación personalizada: no se permiten duplas iguales
            if (Partido.Dupla1Id == Partido.Dupla2Id)
            {
                ModelState.AddModelError("Partido.Dupla2Id", "No se puede jugar un partido con la misma dupla.");
            }

            // Completar campos requeridos manualmente en cada Set
            for (int i = 0; i < Partido.Sets.Count; i++)
            {
                var set = Partido.Sets[i];

                // Establecer relación con Partido
                set.Partido = Partido;
                set.NumeroSet = i + 1;

                // Determinar el ganador
                set.GanadorDuplaId = (set.PuntosDupla1 > set.PuntosDupla2)
                    ? Partido.Dupla1Id
                    : Partido.Dupla2Id;
            }

            // Limpiar ModelState y revalidar
            ModelState.Clear();
            TryValidateModel(Partido);

            if (!ModelState.IsValid)
            {
                Duplas = new SelectList(_context.Duplas, "Id", "Alias");
                return Page();
            }

            _context.Partidos.Add(Partido);
            _context.SaveChanges();

            // ✅ Esta es la única línea clave
            UltimoPartidoId = Partido.Id;
            return RedirectToPage("./Index");
        }

    }
}
