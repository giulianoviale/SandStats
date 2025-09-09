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
            // 1) Validación duplas
            if (Partido.Dupla1Id == Partido.Dupla2Id)
                ModelState.AddModelError("Partido.Dupla2Id", "No se puede jugar un partido con la misma dupla.");

            bool existeD1 = await _context.Duplas.AnyAsync(d => d.Id == Partido.Dupla1Id);
            bool existeD2 = await _context.Duplas.AnyAsync(d => d.Id == Partido.Dupla2Id);
            if (!existeD1 || !existeD2)
                ModelState.AddModelError(string.Empty, "Alguna de las duplas seleccionadas no existe.");

            // 2) Normalizar sets recibidos (evitar nulls)
            Partido.Sets ??= new List<Set>();

            // Tomo solo los sets con al menos un dato cargado
            var setsValidos = Partido.Sets
                .Where(s => s != null && (s.PuntosDupla1 > 0 || s.PuntosDupla2 > 0))
                .ToList();

            // Deben ser 2 o 3 sets
            if (setsValidos.Count < 2)
                ModelState.AddModelError(string.Empty, "Cargá al menos dos sets con puntaje.");

            // 3) Completar cada set y validar no-empate
            for (int i = 0; i < setsValidos.Count; i++)
            {
                var set = setsValidos[i];
                set.Partido = Partido;
                set.NumeroSet = i + 1;

                if (set.PuntosDupla1 == set.PuntosDupla2)
                {
                    ModelState.AddModelError(string.Empty, $"El Set {set.NumeroSet} tiene un empate. Ingresá un ganador.");
                    continue;
                }

                set.GanadorDuplaId = (set.PuntosDupla1 > set.PuntosDupla2)
                    ? Partido.Dupla1Id
                    : Partido.Dupla2Id;
            }

            // Si hay errores de validación hasta acá, volver a la página
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 4) Sets ganados: respetar lo escrito; si quedaron en 0, inferir
            if (Partido.SetsGanadosDupla1 == 0 && Partido.SetsGanadosDupla2 == 0)
            {
                Partido.SetsGanadosDupla1 = setsValidos.Count(s => s.GanadorDuplaId == Partido.Dupla1Id);
                Partido.SetsGanadosDupla2 = setsValidos.Count(s => s.GanadorDuplaId == Partido.Dupla2Id);
            }

            // (Opcional) coherencia: si los escribieron a mano y no coinciden, avisar pero permitir
            var sg1Calc = setsValidos.Count(s => s.GanadorDuplaId == Partido.Dupla1Id);
            var sg2Calc = setsValidos.Count(s => s.GanadorDuplaId == Partido.Dupla2Id);
            if (Partido.SetsGanadosDupla1 != sg1Calc || Partido.SetsGanadosDupla2 != sg2Calc)
            {
                // Solo aviso; si querés bloquear, convertí esto en ModelState error
                TempData["Warn"] = "Los sets ganados no coinciden con los ganadores por set. Se guardará igualmente.";
            }

            // 5) Reemplazar por la lista validada (sin sets vacíos)
            Partido.Sets = setsValidos;

            // 6) Guardar
            try
            {
                _context.Partidos.Add(Partido);
                await _context.SaveChangesAsync();

                UltimoPartidoId = Partido.Id;
                TempData["PartidoIdUltimo"] = Partido.Id;
                TempData["PartidoCreado"] = "1";

                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                // Si llegara una constraint de NOT NULL (ej: GanadorDuplaId), lo mostramos bien
                ModelState.AddModelError(string.Empty, "No se pudo guardar el partido. Revisá que cada set tenga un ganador.");
                // (Opcional) loguear ex.Message
                await OnGet();
                return Page();
            }
        }

    }
}
