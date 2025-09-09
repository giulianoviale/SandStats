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
            // Evitar nulls
            Partido.Sets ??= new List<Set>();

            // Validación base
            if (Partido.Dupla1Id == Partido.Dupla2Id)
                ModelState.AddModelError("Partido.Dupla2Id", "No se puede jugar un partido con la misma dupla.");

            // Deben venir 2 o 3 sets
            if (Partido.Sets.Count < 2 || Partido.Sets.Count > 3)
                ModelState.AddModelError(string.Empty, "Debés cargar 2 o 3 sets.");

            // Si ya hay errores, corto acá
            if (!ModelState.IsValid)
            {
                // No toques Partido.Sets: devolvé la página tal cual para no perder lo cargado
                return Page();
            }

            // ===== Completar datos de cada set + validar ganador =====
            int gana1 = 0, gana2 = 0;
            for (int i = 0; i < Partido.Sets.Count; i++)
            {
                var set = Partido.Sets[i];
                set.Partido = Partido;     // relacionar
                set.NumeroSet = i + 1;

                // Validar “tiene ganador”
                if (set.PuntosDupla1 == set.PuntosDupla2)
                {
                    ModelState.AddModelError(string.Empty, $"El Set {set.NumeroSet} no tiene ganador (empate).");
                    continue;
                }

                // Determinar ganador
                set.GanadorDuplaId = (set.PuntosDupla1 > set.PuntosDupla2)
                    ? Partido.Dupla1Id
                    : Partido.Dupla2Id;

                if (set.GanadorDuplaId == Partido.Dupla1Id) gana1++; else gana2++;
            }

            if (!ModelState.IsValid) return Page();

            // ===== Consistencia con los campos “SetsGanados*” =====
            // (Los mantenemos “cargables”, pero si vienen distintos los corregimos.)
            if (Partido.SetsGanadosDupla1 != gana1 || Partido.SetsGanadosDupla2 != gana2)
            {
                Partido.SetsGanadosDupla1 = gana1;
                Partido.SetsGanadosDupla2 = gana2;
                // Si preferís obligar a coincidir en vez de corregir:
                // ModelState.AddModelError("", "Los sets ganados no coinciden con los resultados de los sets.");
                // return Page();
            }

            // Reglas rápidas: alguien debe ganar 2 sets
            if (gana1 != 2 && gana2 != 2)
            {
                ModelState.AddModelError("", "Un partido válido requiere que una dupla gane 2 sets.");
                return Page();
            }

            try
            {
                _context.Partidos.Add(Partido);
                _context.SaveChanges();

                // Devuelvo al índice (o a detalle)
                UltimoPartidoId = Partido.Id;
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                // Mostrar el error en la vista en lugar de página de error
                ModelState.AddModelError("", "No se pudo guardar el partido. " + ex.Message);
                return Page();
            }
        }


    }
}
