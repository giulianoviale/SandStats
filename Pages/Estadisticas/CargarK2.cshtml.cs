using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using System.Text.RegularExpressions;

namespace SandStats.Pages.Estadisticas
{
    public class CargarK2Model : PageModel
    {
        private readonly ApplicationDbContext _db;
        public CargarK2Model(ApplicationDbContext db) => _db = db;

        [BindProperty(SupportsGet = true)] public int JugadorId { get; set; }
        [BindProperty(SupportsGet = true)] public int PartidoId { get; set; }

        public Jugador? Jugador { get; private set; }
        public string PartidoDescripcion { get; private set; } = "";

        public async Task<IActionResult> OnGetAsync(int jugadorId, int partidoId)
        {
            JugadorId = jugadorId; PartidoId = partidoId;

            var partido = await _db.Partidos
                .Include(p => p.Dupla1).Include(p => p.Dupla2)
                .FirstOrDefaultAsync(p => p.Id == partidoId);
            if (partido is null) return NotFound();
            var sets = partido?.SetsJugados ?? 0;
            PartidoDescripcion = partido.Descripcion ?? $"Partido #{partidoId}";
            Jugador = await _db.Jugadores.FindAsync(jugadorId);

            if (Jugador is null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // borrar cargas previas de este jugador/partido (opcional)
            var prev = await _db.EstadisticaK2
                .Where(e => e.PartidoId == PartidoId && e.JugadorId == JugadorId)
                .ToListAsync();
            if (prev.Count > 0) { _db.EstadisticaK2.RemoveRange(prev); }

            // K2[Fuente][Resultado] = cantidad
            foreach (var key in Request.Form.Keys.Where(k => k.StartsWith("K2[")))
            {
                // K2[Fuente][Resultado]
                var m = Regex.Match(key, @"^K2\[(.+?)\]\[(.+?)\]$");
                if (!m.Success) continue;

                if (!int.TryParse(Request.Form[key], out var cant) || cant <= 0) continue;

                var fuente = Enum.Parse<FuenteK2>(m.Groups[1].Value, true);
                var resultado = Enum.Parse<ResultadoK2>(m.Groups[2].Value, true);

                _db.EstadisticaK2.Add(new EstadisticaK2
                {
                    PartidoId = PartidoId,
                    JugadorId = JugadorId,
                    Fuente = fuente,
                    Resultado = resultado,
                    Cantidad = cant,
                    Scope = ScopeEstadistica.PartidoCompleto // si algún día quieres cierre, acá
                });
            }

            // Extras
            int Try(string k) => int.TryParse(Request.Form[$"Extras[{k}]"], out var v) ? v : 0;
            var extras = new EstadisticaK2
            {
                PartidoId = PartidoId,
                JugadorId = JugadorId,
                Fuente = FuenteK2.PuntosJugados, // dummy para guardar extras en misma tabla (opcional)
                Resultado = ResultadoK2.Positivo, // no se usa
                Cantidad = 0,
                Scope = ScopeEstadistica.PartidoCompleto,
                SetsJugados = Try("SetsJugados"),
                ErroresVarios = Try("ErroresVarios"),
                Agregados = Try("Agregados")
            };
            _db.EstadisticaK2.Add(extras);

            await _db.SaveChangesAsync();
            TempData["Mensaje"] = "K2 guardado correctamente.";
            return RedirectToPage("/Estadisticas/Resumen", new { partidoId = PartidoId });
        }
    }
}
