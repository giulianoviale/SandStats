using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using System.Text.RegularExpressions;

namespace SandStats.Pages.Estadisticas
{
    public class CargarAtaquesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public CargarAtaquesModel(ApplicationDbContext context) => _context = context;

        public Jugador JugadorSeleccionado { get; set; } = default!;
        public List<TipoAcciones> AccionesFiltradas { get; set; } = new();
        public Dictionary<TipoLado, List<TipoAcciones>> AccionesPorLado { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int PartidoId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int JugadorId { get; set; }

        public SelectList? Partidos { get; set; }
        public SelectList? Jugadores { get; set; }
        public string PartidoDescripcion { get; set; } = "";

        // Alcance/segmentación
        [BindProperty(SupportsGet = true)]
        public ScopeEstadistica Scope { get; set; } = ScopeEstadistica.PartidoCompleto;

        [BindProperty(SupportsGet = true)]
        public int? DesdePunto { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? SetNumero { get; set; } // 1, 2 o 3 (tie-break)

        public async Task<IActionResult> OnGetAsync(int jugadorId, int partidoId)
        {
            // Persistimos ids en el modelo
            JugadorId = jugadorId;
            PartidoId = partidoId;

            var partido = await _context.Partidos
                .Include(p => p.Dupla1)
                .Include(p => p.Dupla2)
                .FirstOrDefaultAsync(p => p.Id == partidoId);

            if (partido is null) return NotFound();

            PartidoDescripcion = partido.Descripcion ?? $"Partido #{partidoId}";

            JugadorSeleccionado = await _context.Jugadores.FindAsync(jugadorId)
                                   ?? throw new InvalidOperationException("Jugador no encontrado.");

            // Defaults inteligentes para Cierre
            if (Scope == ScopeEstadistica.Cierre)
            {
                if (!DesdePunto.HasValue)
                    DesdePunto = (SetNumero == 3) ? 11 : 16;
            }
            else
            {
                // En PartidoCompleto no aplican
                DesdePunto = null;
                SetNumero = null;
            }

            // ============================================
            // === Armar acciones por lado (respetando rol)
            // ============================================
            AccionesPorLado = new();

            bool esRol4 = JugadorSeleccionado.RolPrincipal == RolJugador.Rol4;

            // Acciones "normales" (SI tienen Varilla/Entre línea)
            var baseNormales = new List<TipoAcciones>
            {
                TipoAcciones.Atq1, TipoAcciones.Atq2, TipoAcciones.Atq3,
                TipoAcciones.Atq4, TipoAcciones.Atq5, TipoAcciones.Atq6,
                TipoAcciones.Atq7, TipoAcciones.Atq8, TipoAcciones.Atq9
            };

            // Acciones ESPECIALES (SIN V/E)
            var especiales = new[]
            {
                TipoAcciones.Atq2daA1, TipoAcciones.Atq2daA6, TipoAcciones.Atq2daA5,
                TipoAcciones.Varios,   TipoAcciones.PorAtras
            };

            // ==========================
            //  TOQUES POR LADO / ROL
            // ==========================

            // --- Rol 4: lado bueno / atrás ---
            var tlBueno_Rol4 = new[] { TipoAcciones.Tl1, TipoAcciones.Tl9, TipoAcciones.Tl2 }; // 1-9-2
            var tdBueno_Rol4 = new[]
            {
                TipoAcciones.Td3, TipoAcciones.Td4, TipoAcciones.Td5,
                TipoAcciones.Td6, TipoAcciones.Td7, TipoAcciones.Td8
            };

            var tlAtras_Rol4 = new[] { TipoAcciones.Tl5, TipoAcciones.Tl7, TipoAcciones.Tl4 }; // 5-7-4
            var tdAtras_Rol4 = new[]
            {
                TipoAcciones.Td1, TipoAcciones.Td2, TipoAcciones.Td3,
                TipoAcciones.Td6, TipoAcciones.Td8, TipoAcciones.Td9
            };

            // --- Rol 2: se invierte Bueno <-> Atrás ---
            var tlBueno_Rol2 = tlAtras_Rol4;
            var tdBueno_Rol2 = tdAtras_Rol4;

            var tlAtras_Rol2 = tlBueno_Rol4;
            var tdAtras_Rol2 = tdBueno_Rol4;

            // --- LADO MEDIO parametrizado por rol ---
            // Rol 4: TL = 1,9,2 ; TD = 3,4,5,6,7,8
            var tlMedio_Rol4 = new[]
            {
                TipoAcciones.Tl1, TipoAcciones.Tl9, TipoAcciones.Tl2
            };
            var tdMedio_Rol4 = new[]
            {
                TipoAcciones.Td3, TipoAcciones.Td4, TipoAcciones.Td5,
                TipoAcciones.Td6, TipoAcciones.Td7, TipoAcciones.Td8
            };

            // Rol 2: TL = 5,7,4 ; TD = 1,2,3,6,8,9
            var tlMedio_Rol2 = new[]
            {
                TipoAcciones.Tl5, TipoAcciones.Tl7, TipoAcciones.Tl4
            };
            var tdMedio_Rol2 = new[]
            {
                TipoAcciones.Td1, TipoAcciones.Td2, TipoAcciones.Td3,
                TipoAcciones.Td6, TipoAcciones.Td8, TipoAcciones.Td9
            };

            foreach (TipoLado lado in Enum.GetValues(typeof(TipoLado)))
            {
                // Empezamos por las normales (Atq1..Atq9)
                var acciones = new List<TipoAcciones>(baseNormales);

                if (lado == TipoLado.Bueno)
                {
                    if (esRol4)
                        acciones.AddRange(tlBueno_Rol4.Concat(tdBueno_Rol4));
                    else
                        acciones.AddRange(tlBueno_Rol2.Concat(tdBueno_Rol2));
                }
                else if (lado == TipoLado.Medio)
                {
                    if (esRol4)
                        acciones.AddRange(tlMedio_Rol4.Concat(tdMedio_Rol4));
                    else
                        acciones.AddRange(tlMedio_Rol2.Concat(tdMedio_Rol2));
                }
                else // Atrás
                {
                    if (esRol4)
                        acciones.AddRange(tlAtras_Rol4.Concat(tdAtras_Rol4));
                    else
                        acciones.AddRange(tlAtras_Rol2.Concat(tdAtras_Rol2));
                }

                // SIEMPRE al final las ESPECIALES (sin V/E)
                acciones.AddRange(especiales);

                AccionesPorLado[lado] = acciones;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var jugadorId = JugadorId;
            var partidoId = PartidoId;

            // Alcance/segmentación desde el binding
            var scope = Scope;
            int? desde = null;
            int? set = null;

            if (scope == ScopeEstadistica.Cierre)
            {
                set = SetNumero;
                desde = DesdePunto ?? (set == 3 ? 11 : 16);
            }

            // --- helper: normaliza el nombre del enum ResultadoAtaque según el lado ---
            // Para Lado Medio: si viene sin sufijo, agrego 'V'; si viene con V/E, fuerzo 'V'
            string NormalizarResultado(TipoLado lado, string resultadoStr)
            {
                var r = (resultadoStr ?? string.Empty).Trim();

                if (lado != TipoLado.Medio) return r;

                if (r.EndsWith("V", StringComparison.OrdinalIgnoreCase))
                    return r; // ya está en V

                if (r.EndsWith("E", StringComparison.OrdinalIgnoreCase))
                    return r[..^1] + "V"; // cambia E -> V

                return r + "V";
            }

            // --------- Impactos NORMALES (con/sin V/E según lado) ----------
            foreach (var key in Request.Form.Keys)
            {
                if (!key.StartsWith("Impactos[", StringComparison.Ordinal)) continue;

                // Impactos[Lado][Accion][Resultado]
                var m = Regex.Match(key, @"^Impactos\[(.+?)\]\[(.+?)\]\[(.+?)\]$");
                if (!m.Success) continue;

                if (!int.TryParse(Request.Form[key], out var cantidad) || cantidad <= 0)
                    continue;

                try
                {
                    var lado = Enum.Parse<TipoLado>(m.Groups[1].Value, true);
                    var accion = Enum.Parse<TipoAcciones>(m.Groups[2].Value, true);

                    var resultadoNombre = NormalizarResultado(lado, m.Groups[3].Value);
                    if (!Enum.TryParse<ResultadoAtaque>(resultadoNombre, true, out var resultado))
                        continue;

                    _context.EstadisticaAtaque.Add(new EstadisticaAtaque
                    {
                        JugadorId = jugadorId,
                        PartidoId = partidoId,
                        Lado = lado,
                        Accion = accion,
                        Resultado = resultado,
                        Cantidad = cantidad,
                        Scope = scope,
                        DesdePunto = (scope == ScopeEstadistica.Cierre) ? desde : null,
                        SetNumero = (scope == ScopeEstadistica.Cierre) ? set : null
                    });
                }
                catch
                {
                    // opcional: log
                }
            }

            // --------- Impactos ESPECIALES (sin V/E) ----------
            foreach (var key in Request.Form.Keys)
            {
                if (!key.StartsWith("ImpactosEspeciales[", StringComparison.Ordinal)) continue;

                // ImpactosEspeciales[Lado][Accion][Resultado]
                var m = Regex.Match(key, @"^ImpactosEspeciales\[(.+?)\]\[(.+?)\]\[(.+?)\]$");
                if (!m.Success) continue;

                if (!int.TryParse(Request.Form[key], out var cantidad) || cantidad <= 0)
                    continue;

                try
                {
                    var lado = Enum.Parse<TipoLado>(m.Groups[1].Value, true);
                    var accion = Enum.Parse<TipoAcciones>(m.Groups[2].Value, true);

                    var resultadoNombre = NormalizarResultado(lado, m.Groups[3].Value);
                    if (!Enum.TryParse<ResultadoAtaque>(resultadoNombre, true, out var resultado))
                        continue;

                    _context.EstadisticaAtaque.Add(new EstadisticaAtaque
                    {
                        JugadorId = jugadorId,
                        PartidoId = partidoId,
                        Lado = lado,
                        Accion = accion,
                        Resultado = resultado,
                        Cantidad = cantidad,
                        Scope = scope,
                        DesdePunto = (scope == ScopeEstadistica.Cierre) ? desde : null,
                        SetNumero = (scope == ScopeEstadistica.Cierre) ? set : null
                    });
                }
                catch
                {
                    // opcional: log
                }
            }

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Estadísticas de ataque guardadas correctamente.";
            return RedirectToPage("/Estadisticas/Resumen", new { partidoId });
        }
    }
}
