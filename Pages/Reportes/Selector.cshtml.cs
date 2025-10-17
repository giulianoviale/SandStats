using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;

namespace SandStats.Pages.Reportes
{
    // ===== ViewModels =====
    public class FiltroVM
    {
        public int? DuplaId { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public bool IncluirAmistosos { get; set; }

        public bool SoloConRecepcion { get; set; }
        public bool SoloConAtaque { get; set; }
        public bool SoloConK2 { get; set; }

        public int? UltimosN { get; set; }

        public bool IncluirCierre { get; set; }
        public int? SetNumero { get; set; }

        public Clima? Clima { get; set; }
    }

    public class PartidoVM
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Torneo { get; set; } = "-";
        public string Rival { get; set; } = "-";
        public int CantSets { get; set; }

        public bool TieneRecepcion { get; set; }
        public bool TieneAtaque { get; set; }
        public bool TieneK2 { get; set; }

        public Clima Clima { get; set; }
    }

    // ===== PageModel =====
    public class SelectorModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public SelectorModel(ApplicationDbContext context) => _context = context;

        [BindProperty] public FiltroVM Filtro { get; set; } = new();
        [BindProperty] public List<int> Seleccionados { get; set; } = new();

        public List<SelectListItem> Duplas { get; set; } = new();
        public List<SelectListItem> Climas { get; set; } = new();
        public List<PartidoVM>? Partidos { get; set; }
        public bool BusquedaRealizada { get; set; }

        public async Task OnGet()
        {
            await CargarDuplasAsync();
            await CargarClimasAsync();
        }

        public async Task<IActionResult> OnPostBuscar()
        {
            await CargarDuplasAsync();
            await CargarClimasAsync();
            BusquedaRealizada = true;

            if (!(Filtro.DuplaId > 0))
            {
                Partidos = new();
                ModelState.AddModelError(string.Empty, "Seleccioná una dupla.");
                return Page();
            }

            try
            {
                var q = _context.Partidos
                    .AsNoTracking()
                    .Include(p => p.Dupla1).Include(p => p.Dupla2)
                    .Include(p => p.Sets)
                    .Where(p => p.Dupla1Id == Filtro.DuplaId || p.Dupla2Id == Filtro.DuplaId);

                // ------ Fecha (robusta para EF/Render) ------
                var desde = Filtro.Desde?.Date;
                var hastaExcl = Filtro.Hasta?.Date.AddDays(1); // límite superior exclusivo

                if (desde.HasValue)
                    q = q.Where(p => p.Fecha >= desde.Value);

                if (hastaExcl.HasValue)
                    q = q.Where(p => p.Fecha < hastaExcl.Value);

                // Clima
                if (Filtro.Clima.HasValue)
                    q = q.Where(p => p.Clima == Filtro.Clima.Value);

                // Amistosos (si existe la columna)
                var entidad = _context.Model.FindEntityType(typeof(Partido));
                bool hasEsAmistoso = entidad?.FindProperty("EsAmistoso") != null;
                if (!Filtro.IncluirAmistosos && hasEsAmistoso)
                    q = q.Where(p => !EF.Property<bool>(p, "EsAmistoso"));

                // Módulos
                if (Filtro.SoloConRecepcion)
                    q = q.Where(p => _context.EstadisticaRecepcion.Any(e => e.PartidoId == p.Id));
                if (Filtro.SoloConAtaque)
                    q = q.Where(p => _context.EstadisticaAtaque.Any(e => e.PartidoId == p.Id));
                if (Filtro.SoloConK2)
                    q = q.Where(p => _context.EstadisticaK2.Any(e => e.PartidoId == p.Id));

                q = q.OrderByDescending(p => p.Fecha);

                if (Filtro.UltimosN.HasValue && Filtro.UltimosN.Value > 0)
                    q = q.Take(Filtro.UltimosN.Value);

                Partidos = await q
                    .Select(p => new PartidoVM
                    {
                        Id = p.Id,
                        Fecha = p.Fecha,
                        Torneo = p.Torneo ?? "-",
                        Rival = p.Dupla1Id == Filtro.DuplaId ? (p.Dupla2!.Alias) : (p.Dupla1!.Alias),
                        CantSets = p.Sets.Count,
                        TieneRecepcion = _context.EstadisticaRecepcion.Any(e => e.PartidoId == p.Id),
                        TieneAtaque = _context.EstadisticaAtaque.Any(e => e.PartidoId == p.Id),
                        TieneK2 = _context.EstadisticaK2.Any(e => e.PartidoId == p.Id),
                        Clima = p.Clima
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[Selector/Buscar] Error: {ex}");
                ModelState.AddModelError(string.Empty, "Hubo un error al aplicar los filtros de fecha. Probá nuevamente.");
                Partidos = new();
            }

            return Page();
        }

        public IActionResult OnPostVerReportes()
        {
            if (!(Filtro.DuplaId > 0))
            {
                ModelState.AddModelError(string.Empty, "Seleccioná una dupla.");
                BusquedaRealizada = true;
                return Page();
            }

            if (Seleccionados == null || Seleccionados.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Elegí al menos un partido.");
                BusquedaRealizada = true;
                return Page();
            }

            return RedirectToPage("/Estadisticas/ReporteDupla", new
            {
                DuplaId = Filtro.DuplaId,
                PartidoIds = Seleccionados,
                IncluirCierre = Filtro.IncluirCierre,
                SetNumero = Filtro.SetNumero,
                Clima = Filtro.Clima
            });
        }

        // ===== Helpers =====

        // 🔹 Alias visible en el combo
        private async Task CargarDuplasAsync()
        {
            var duplasUi = await _context.Duplas
                .AsNoTracking()
                .Select(d => new { d.Id, d.Alias })
                .OrderBy(x => x.Alias)
                .ToListAsync();

            Duplas = duplasUi
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = string.IsNullOrWhiteSpace(x.Alias) ? $"Dupla {x.Id}" : x.Alias
                })
                .ToList();
        }

        private Task CargarClimasAsync()
        {
            Climas = Enum.GetValues(typeof(Clima))
                .Cast<Clima>()
                .Select(c => new SelectListItem
                {
                    Value = ((int)c).ToString(),
                    Text = c.ToString(),
                    Selected = Filtro.Clima.HasValue && Filtro.Clima.Value == c
                })
                .ToList();
            return Task.CompletedTask;
        }
    }
}
