using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using SandStats.Models.SandStats.Models; // EstadisticaRecepcion
using System.Text.RegularExpressions;

namespace SandStats.Pages.Estadisticas
{
    public class ResumenModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ResumenModel(ApplicationDbContext context) => _context = context;

        public Partido Partido { get; set; } = default!;
        public List<Dupla> DuplasDelPartido => new() { Partido.Dupla1!, Partido.Dupla2! };

        // Stats del partido (AsNoTracking)
        public List<EstadisticaAtaque> EstadisticasAtaque { get; set; } = new();
        public List<SandStats.Models.SandStats.Models.EstadisticaRecepcion> EstadisticasRecepcion { get; set; } = new();
        public List<EstadisticaK2> EstadisticasK2 { get; set; } = new();

        public int SetsJugados { get; private set; } = 3;
        public Dictionary<int, List<Partido>> PartidosPorDupla { get; private set; } = new();
        // NUEVO: para renderizar los valores actuales
        public Dictionary<int, VideoLinksJugadorPartido> VideoLinksPorJugador { get; private set; } = new();

       
        public async Task<IActionResult> OnGetAsync(int partidoId)
        {
            if (partidoId <= 0)
                return RedirectToPage("/Partidos/Index");

            Partido = await _context.Partidos
                .Include(p => p.Dupla1).ThenInclude(d => d.Jugador1)
                .Include(p => p.Dupla1).ThenInclude(d => d.Jugador2)
                .Include(p => p.Dupla2).ThenInclude(d => d.Jugador1)
                .Include(p => p.Dupla2).ThenInclude(d => d.Jugador2)
                .FirstOrDefaultAsync(p => p.Id == partidoId)
                ?? throw new Exception("Partido no encontrado.");

            // … dentro de OnGetAsync, después de cargar Partido/Duplas:
            var jIds = DuplasDelPartido
                .SelectMany(d => new[] { d.Jugador1Id, d.Jugador2Id })
                .Distinct()
                .ToList();

            VideoLinksPorJugador = await _context.VideoLinksJugadorPartido
                .Where(v => v.PartidoId == Partido.Id && jIds.Contains(v.JugadorId))
                .ToDictionaryAsync(v => v.JugadorId, v => v);

            EstadisticasAtaque = await _context.EstadisticaAtaque
                .AsNoTracking()
                .Where(e => e.PartidoId == partidoId)
                .ToListAsync();

            EstadisticasRecepcion = await _context.EstadisticaRecepcion
                .AsNoTracking()
                .Where(e => e.PartidoId == partidoId)
                .Cast<SandStats.Models.SandStats.Models.EstadisticaRecepcion>()
                .ToListAsync();

            EstadisticasK2 = await _context.EstadisticaK2
                .AsNoTracking()
                .Where(e => e.PartidoId == partidoId)
                .ToListAsync();

            SetsJugados = await DetectarSetsJugadosAsync(partidoId, Partido, EstadisticasAtaque);
            await CargarPartidosHistoricosPorDupla(Partido.Dupla1Id);
            await CargarPartidosHistoricosPorDupla(Partido.Dupla2Id);

            return Page();
        }

        // ====== Guardar Observaciones + VideoUrl (AJAX) ======
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostGuardarObservacionesAsync(int partidoId, string? observaciones, string? videoUrl)
        {
            var partido = await _context.Partidos.FirstOrDefaultAsync(p => p.Id == partidoId);
            if (partido == null)
                return new JsonResult(new { ok = false, error = "Partido no encontrado" }) { StatusCode = 404 };

            // Observaciones (recortar a 1000 por seguridad)
            if (!string.IsNullOrWhiteSpace(observaciones))
                observaciones = observaciones.Length > 1000 ? observaciones[..1000] : observaciones;
            partido.Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim();

            // VideoUrl: normalizar y validar básica
            string? normalized = NormalizeUrl(videoUrl);
            if (!string.IsNullOrEmpty(normalized) && !Uri.IsWellFormedUriString(normalized, UriKind.Absolute))
                return new JsonResult(new { ok = false, error = "URL inválida" }) { StatusCode = 400 };

            partido.VideoUrl = string.IsNullOrWhiteSpace(normalized) ? null : normalized;

            await _context.SaveChangesAsync();

            var savedAt = DateTime.Now.ToString("HH:mm");
            return new JsonResult(new { ok = true, savedAt });
        }

        private static string? NormalizeUrl(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;
            var s = raw.Trim();
            // Si pega "www.youtube..." o "youtube.com/..." sin protocolo, anteponer https://
            if (!Regex.IsMatch(s, @"^https?://", RegexOptions.IgnoreCase))
                s = "https://" + s;
            return s;
        }

        // Embed de YouTube/Vimeo (para preview)
        public string? TryGetEmbedUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            var u = url.Trim();

            // YouTube formatos comunes
            // https://www.youtube.com/watch?v=VIDEOID
            var m = Regex.Match(u, @"(?:youtube\.com/watch\?v=|youtu\.be/)([A-Za-z0-9_-]{6,})", RegexOptions.IgnoreCase);
            if (m.Success)
                return $"https://www.youtube.com/embed/{m.Groups[1].Value}";

            // Vimeo: https://vimeo.com/ID
            var v = Regex.Match(u, @"vimeo\.com/(\d+)", RegexOptions.IgnoreCase);
            if (v.Success)
                return $"https://player.vimeo.com/video/{v.Groups[1].Value}";

            return null; // otros hosts: no embebemos, sólo botón "Abrir"
        }

        // ========= Helpers existentes =========
        public static int DesdePorDefecto(int? setNumero) => (setNumero == 3) ? 11 : 16;

        public bool TieneAtaqueCompleto(int jugadorId) =>
            EstadisticasAtaque.Any(e => e.PartidoId == Partido.Id && e.JugadorId == jugadorId &&
                                        e.Scope == ScopeEstadistica.PartidoCompleto && e.SetNumero == null);

        public bool TieneAtaqueCierre(int jugadorId, int setNumero) =>
            EstadisticasAtaque.Any(e => e.PartidoId == Partido.Id && e.JugadorId == jugadorId &&
                                        e.Scope == ScopeEstadistica.Cierre && e.SetNumero == setNumero);

        public bool TieneRecepcionCompleto(int jugadorId) =>
            EstadisticasRecepcion.Any(e => e.PartidoId == Partido.Id && e.JugadorId == jugadorId &&
                                           e.Scope == ScopeEstadistica.PartidoCompleto && e.SetNumero == null);

        public bool TieneRecepcionCierre(int jugadorId, int setNumero) =>
            EstadisticasRecepcion.Any(e => e.PartidoId == Partido.Id && e.JugadorId == jugadorId &&
                                           e.Scope == ScopeEstadistica.Cierre && e.SetNumero == setNumero);

        public bool TieneK2Completo(int jugadorId) =>
            EstadisticasK2.Any(e => e.PartidoId == Partido.Id && e.JugadorId == jugadorId);

        public bool DuplaTieneDatosEnEstePartido(Dupla dupla)
        {
            var j1 = dupla.Jugador1Id;
            var j2 = dupla.Jugador2Id;

            bool atq = EstadisticasAtaque.Any(e => e.PartidoId == Partido.Id && (e.JugadorId == j1 || e.JugadorId == j2));
            bool rec = EstadisticasRecepcion.Any(e => e.PartidoId == Partido.Id && (e.JugadorId == j1 || e.JugadorId == j2));
            bool k2 = EstadisticasK2.Any(e => e.PartidoId == Partido.Id && (e.JugadorId == j1 || e.JugadorId == j2));

            return atq || rec || k2;
        }

        private async Task<int> DetectarSetsJugadosAsync(int partidoId, Partido partido, List<EstadisticaAtaque> atq)
        {
            var setsRegistrados = await _context.Sets
                .AsNoTracking()
                .Where(s => s.PartidoId == partidoId)
                .CountAsync();
            if (setsRegistrados is >= 2 and <= 3) return setsRegistrados;

            if (!string.IsNullOrWhiteSpace(partido.Resultado))
            {
                var m = Regex.Match(partido.Resultado, @"(?<a>\d)\s*[-–]\s*(?<b>\d)");
                if (m.Success && int.TryParse(m.Groups["a"].Value, out var a) && int.TryParse(m.Groups["b"].Value, out var b))
                {
                    var total = a + b;
                    if (total == 3) return 3;
                    if (total == 2) return 2;
                }
            }

            var cierres = atq
                .Where(e => e.Scope == ScopeEstadistica.Cierre && e.SetNumero.HasValue)
                .Select(e => e.SetNumero!.Value)
                .Distinct()
                .ToList();

            if (cierres.Contains(3)) return 3;
            if (cierres.Contains(1) || cierres.Contains(2)) return 2;
            return 3;
        }

        private async Task CargarPartidosHistoricosPorDupla(int duplaId)
        {
            var lista = await _context.Partidos
                .AsNoTracking()
                .Where(p => p.Dupla1Id == duplaId || p.Dupla2Id == duplaId)
                .Include(p => p.Dupla1)
                .Include(p => p.Dupla2)
                .OrderByDescending(p => p.Fecha)
                .Take(20)
                .ToListAsync();

            PartidosPorDupla[duplaId] = lista;
        }
        //linkvideo
        public async Task<IActionResult> OnPostGuardarLinksJugador(int partidoId, int jugadorId, string? linkK1, string? linkK2, string? linkSaque)
        {
            var row = await _context.VideoLinksJugadorPartido
                .FirstOrDefaultAsync(v => v.PartidoId == partidoId && v.JugadorId == jugadorId);

            if (row == null)
            {
                row = new VideoLinksJugadorPartido
                {
                    PartidoId = partidoId,
                    JugadorId = jugadorId
                };
                _context.VideoLinksJugadorPartido.Add(row);
            }

            row.LinkK1 = string.IsNullOrWhiteSpace(linkK1) ? null : linkK1.Trim();
            row.LinkK2 = string.IsNullOrWhiteSpace(linkK2) ? null : linkK2.Trim();
            row.LinkSaque = string.IsNullOrWhiteSpace(linkSaque) ? null : linkSaque.Trim();

            await _context.SaveChangesAsync();

            return new JsonResult(new { ok = true, id = row.Id });
        }

    }
}
