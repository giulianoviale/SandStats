using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;

namespace SandStats.Pages.Reportes
{
    // ===================== ViewModels =====================
    public class FiltroVM
    {
        public int? DuplaId { get; set; }
        public DateTime? Desde { get; set; }       // viene como Date (Kind = Unspecified)
        public DateTime? Hasta { get; set; }       // idem
        public bool IncluirAmistosos { get; set; }

        // módulos
        public bool SoloConRecepcion { get; set; }
        public bool SoloConAtaque { get; set; }
        public bool SoloConK2 { get; set; }

        // presets
        public int? UltimosN { get; set; }

        // scope para ReporteDupla
        public bool IncluirCierre { get; set; }
        public int? SetNumero { get; set; }

        // clima
        public Clima? Clima { get; set; }
    }

    public class PartidoVM
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Torneo { get; set; } = "-";
        public string Rival { get; set; } = "-";
        public int CantSets { get; set; }
        public Clima Clima { get; set; }

        // ===== Estados por jugador de la dupla seleccionada =====
        public bool RecepJ1 { get; set; }
        public bool RecepJ2 { get; set; }
        public bool AtqJ1 { get; set; }
        public bool AtqJ2 { get; set; }
        public bool K2J1 { get; set; }
        public bool K2J2 { get; set; }

        // ===== Helpers de agregación (para la vista) =====
        public bool RecepAmbos => RecepJ1 && RecepJ2;
        public bool RecepAlguno => RecepJ1 || RecepJ2;

        public bool AtqAmbos => AtqJ1 && AtqJ2;
        public bool AtqAlguno => AtqJ1 || AtqJ2;

        public bool K2Ambos => K2J1 && K2J2;
        public bool K2Alguno => K2J1 || K2J2;

        // Compatibilidad con nombres viejos (si algo más los usa)
        public bool TieneRecepcion => RecepAlguno;
        public bool TieneAtaque => AtqAlguno;
        public bool TieneK2 => K2Alguno;
    }

    // ===================== PageModel =====================
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

            // ================= Cargamos la dupla seleccionada =================
            var dupla = await _context.Duplas
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == Filtro.DuplaId.Value);

            if (dupla == null)
            {
                Partidos = new();
                ModelState.AddModelError(string.Empty, "Dupla no encontrada.");
                return Page();
            }

            int j1Id = dupla.Jugador1Id;
            int j2Id = dupla.Jugador2Id;

            // ================= Base query de partidos =================
            var q = _context.Partidos
                .AsNoTracking()
                .Include(p => p.Dupla1).Include(p => p.Dupla2)
                .Include(p => p.Sets)
                .Where(p => p.Dupla1Id == Filtro.DuplaId || p.Dupla2Id == Filtro.DuplaId);

            // ======== Filtro de Fechas (UTC-safe) ========
            if (Filtro.Desde.HasValue)
            {
                var desdeUtc = ToUtcStartOfDay(Filtro.Desde.Value);
                q = q.Where(p => p.Fecha >= desdeUtc);
            }
            if (Filtro.Hasta.HasValue)
            {
                var hastaExclUtc = ToUtcStartOfDay(Filtro.Hasta.Value).AddDays(1);
                q = q.Where(p => p.Fecha < hastaExclUtc);
            }

            // Clima
            if (Filtro.Clima.HasValue)
                q = q.Where(p => p.Clima == Filtro.Clima.Value);

            // “Amistosos” (columna sombra opcional)
            var entidad = _context.Model.FindEntityType(typeof(Partido));
            bool hasEsAmistoso = entidad?.FindProperty("EsAmistoso") != null;
            if (!Filtro.IncluirAmistosos && hasEsAmistoso)
                q = q.Where(p => !EF.Property<bool>(p, "EsAmistoso"));

            // ========= Filtros "Solo con ..." POR DUPLA =========
            if (Filtro.SoloConRecepcion)
            {
                q = q.Where(p =>
                    _context.EstadisticaRecepcion.Any(e =>
                        e.PartidoId == p.Id &&
                        (e.JugadorId == j1Id || e.JugadorId == j2Id) &&
                        e.Scope == ScopeEstadistica.PartidoCompleto &&
                        e.SetNumero == null));
            }

            if (Filtro.SoloConAtaque)
            {
                q = q.Where(p =>
                    _context.EstadisticaAtaque.Any(e =>
                        e.PartidoId == p.Id &&
                        (e.JugadorId == j1Id || e.JugadorId == j2Id) &&
                        e.Scope == ScopeEstadistica.PartidoCompleto &&
                        e.SetNumero == null));
            }

            if (Filtro.SoloConK2)
            {
                q = q.Where(p =>
                    _context.EstadisticaK2.Any(e =>
                        e.PartidoId == p.Id &&
                        (e.JugadorId == j1Id || e.JugadorId == j2Id)));
            }

            // Orden + límite
            q = q.OrderByDescending(p => p.Fecha);

            if (Filtro.UltimosN.HasValue && Filtro.UltimosN.Value > 0)
                q = q.Take(Filtro.UltimosN.Value);

            // ================= Ejecutamos query de partidos =================
            var partidosDb = await q.ToListAsync();

            if (partidosDb.Count == 0)
            {
                Partidos = new();
                return Page();
            }

            var partidoIds = partidosDb.Select(p => p.Id).ToList();

            // ================= Stats por jugador de la dupla =================
            var estRecep = await _context.EstadisticaRecepcion
                .AsNoTracking()
                .Where(e =>
                    partidoIds.Contains(e.PartidoId) &&
                    (e.JugadorId == j1Id || e.JugadorId == j2Id) &&
                    e.Scope == ScopeEstadistica.PartidoCompleto &&
                    e.SetNumero == null)
                .ToListAsync();

            var estAtaque = await _context.EstadisticaAtaque
                .AsNoTracking()
                .Where(e =>
                    partidoIds.Contains(e.PartidoId) &&
                    (e.JugadorId == j1Id || e.JugadorId == j2Id) &&
                    e.Scope == ScopeEstadistica.PartidoCompleto &&
                    e.SetNumero == null)
                .ToListAsync();

            var estK2 = await _context.EstadisticaK2
                .AsNoTracking()
                .Where(e =>
                    partidoIds.Contains(e.PartidoId) &&
                    (e.JugadorId == j1Id || e.JugadorId == j2Id))
                .ToListAsync();

            // ================= Armamos los VM =================
            Partidos = partidosDb
                .Select(p => new PartidoVM
                {
                    Id = p.Id,
                    Fecha = p.Fecha,
                    Torneo = p.Torneo ?? "-",
                    Rival = p.Dupla1Id == Filtro.DuplaId ? (p.Dupla2!.Alias) : (p.Dupla1!.Alias),
                    CantSets = p.Sets.Count,
                    Clima = p.Clima,

                    RecepJ1 = estRecep.Any(e => e.PartidoId == p.Id && e.JugadorId == j1Id),
                    RecepJ2 = estRecep.Any(e => e.PartidoId == p.Id && e.JugadorId == j2Id),

                    AtqJ1 = estAtaque.Any(e => e.PartidoId == p.Id && e.JugadorId == j1Id),
                    AtqJ2 = estAtaque.Any(e => e.PartidoId == p.Id && e.JugadorId == j2Id),

                    K2J1 = estK2.Any(e => e.PartidoId == p.Id && e.JugadorId == j1Id),
                    K2J2 = estK2.Any(e => e.PartidoId == p.Id && e.JugadorId == j2Id)
                })
                .ToList();

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

        // ===================== Helpers =====================

        private async Task CargarDuplasAsync()
        {
            // Mostramos ALIAS en el selector
            var duplasUi = await _context.Duplas
                .AsNoTracking()
                .Select(d => new { d.Id, d.Alias })
                .OrderBy(x => x.Alias)
                .ToListAsync();

            Duplas = duplasUi
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Alias
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

        /// <summary>
        /// Toma un DateTime (generalmente Kind=Unspecified del date input)
        /// y lo convierte a 00:00 UTC del mismo “día” para comparar en Render.
        /// </summary>
        private static DateTime ToUtcStartOfDay(DateTime d)
        {
            var unspecifiedMidnight = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, DateTimeKind.Unspecified);
            return DateTime.SpecifyKind(unspecifiedMidnight, DateTimeKind.Utc);
        }
    }
}
