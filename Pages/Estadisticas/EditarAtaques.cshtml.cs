using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using System.Text.RegularExpressions;

namespace SandStats.Pages.Estadisticas
{
    public class EditarAtaquesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EditarAtaquesModel(ApplicationDbContext context) => _context = context;

        // ===== Datos de cabecera / binding =====
        [BindProperty(SupportsGet = true)] public int PartidoId { get; set; }
        [BindProperty(SupportsGet = true)] public int JugadorId { get; set; }

        public string PartidoDescripcion { get; set; } = "";
        public Jugador JugadorSeleccionado { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public ScopeEstadistica Scope { get; set; } = ScopeEstadistica.PartidoCompleto;

        [BindProperty(SupportsGet = true)] public int? DesdePunto { get; set; }
        [BindProperty(SupportsGet = true)] public int? SetNumero { get; set; }

        // ====== Lo que usa la vista ======
        public Dictionary<TipoLado, List<TipoAcciones>> AccionesPorLado { get; private set; } = new();

        // valores actuales para precargar
        private Dictionary<(TipoLado, TipoAcciones, ResultadoAtaque), int> currentVE = new();
        private Dictionary<(TipoLado, TipoAcciones, ResultadoAtaque), int> currentESP = new();

        // Helpers que llama la vista para rellenar inputs
        public int GetVE(TipoLado lado, TipoAcciones acc, ResultadoAtaque res)
            => currentVE.TryGetValue((lado, acc, res), out var v) ? v : 0;

        public int GetEsp(TipoLado lado, TipoAcciones acc, ResultadoAtaque resSimpleV)
            => currentESP.TryGetValue((lado, acc, resSimpleV), out var v) ? v : 0;

        public async Task<IActionResult> OnGetAsync(int jugadorId, int partidoId)
        {
            JugadorId = jugadorId;
            PartidoId = partidoId;

            var partido = await _context.Partidos
                .Include(p => p.Dupla1).Include(p => p.Dupla2)
                .FirstOrDefaultAsync(p => p.Id == partidoId);
            if (partido is null) return NotFound();

            PartidoDescripcion = partido.Descripcion ?? $"Partido #{partidoId}";

            JugadorSeleccionado = await _context.Jugadores.FindAsync(jugadorId)
                                    ?? throw new InvalidOperationException("Jugador no encontrado.");

            // defaults si es Cierre
            if (Scope == ScopeEstadistica.Cierre)
            {
                if (!DesdePunto.HasValue)
                    DesdePunto = (SetNumero == 3) ? 11 : 16;
            }
            else
            {
                DesdePunto = null;
                SetNumero = null;
            }

            // ====== Armar acciones por lado (igual a Cargar) ======
            AccionesPorLado = BuildAccionesPorLado(JugadorSeleccionado);

            // ====== Precarga de valores existentes ======
            var q = _context.EstadisticaAtaque.AsNoTracking()
                    .Where(e => e.PartidoId == PartidoId && e.JugadorId == JugadorId);

            q = FiltrarPorScope(q, Scope, SetNumero);

            var lista = await q
                .Select(e => new { e.Lado, e.Accion, e.Resultado, e.Cantidad })
                .ToListAsync();

            currentVE.Clear();
            currentESP.Clear();

            foreach (var r in lista)
            {
                // especiales (sin V/E) => levantamos en la variante V
                bool esEspecial = EsEspecial(r.Accion);
                if (esEspecial)
                {
                    var key = (r.Lado, r.Accion, NormalizarASimpleV(r.Resultado));
                    currentESP[key] = (currentESP.TryGetValue(key, out var ant) ? ant : 0) + r.Cantidad;
                }
                else
                {
                    var key = (r.Lado, r.Accion, r.Resultado);
                    currentVE[key] = (currentVE.TryGetValue(key, out var ant) ? ant : 0) + r.Cantidad;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1) BORRAR todo lo que aplique al mismo alcance
            var qDel = _context.EstadisticaAtaque
                        .Where(e => e.PartidoId == PartidoId && e.JugadorId == JugadorId);
            qDel = FiltrarPorScope(qDel, Scope, SetNumero);
            _context.EstadisticaAtaque.RemoveRange(qDel);
            await _context.SaveChangesAsync();

            // 2) INSERTAR lo nuevo
            var scope = Scope;
            int? desde = null;
            int? set = null;
            if (scope == ScopeEstadistica.Cierre)
            {
                set = SetNumero;
                desde = DesdePunto ?? (set == 3 ? 11 : 16);
            }

            // --- helper: normaliza nombre de enum ResultadoAtaque según lado ---
            // En Lado Medio: fuerza siempre la variante ...V (si viene ...E o sin sufijo)
            string NormalizarResultado(TipoLado lado, string resultadoStr)
            {
                var r = (resultadoStr ?? string.Empty).Trim();

                if (lado != TipoLado.Medio)
                    return r; // Bueno/Atrás: respetar V/E

                if (r.EndsWith("V", StringComparison.OrdinalIgnoreCase))
                    return r;

                if (r.EndsWith("E", StringComparison.OrdinalIgnoreCase))
                    return r[..^1] + "V"; // cambia E -> V

                return r + "V"; // sin sufijo -> agrega V
            }

            // ========== NORMALES (con/sin V/E según lado) ==========
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
                    if (!Enum.TryParse<TipoLado>(m.Groups[1].Value, true, out var lado)) continue;
                    if (!Enum.TryParse<TipoAcciones>(m.Groups[2].Value, true, out var accion)) continue;

                    var resultadoNombre = NormalizarResultado(lado, m.Groups[3].Value);
                    if (!Enum.TryParse<ResultadoAtaque>(resultadoNombre, true, out var resultado)) continue;

                    _context.EstadisticaAtaque.Add(new EstadisticaAtaque
                    {
                        JugadorId = JugadorId,
                        PartidoId = PartidoId,
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

            // ========== ESPECIALES (sin V/E) – usamos variante V ==========
            foreach (var key in Request.Form.Keys)
            {
                if (!key.StartsWith("ImpactosEspeciales[", StringComparison.Ordinal)) continue;

                // ImpactosEspeciales[Lado][Accion][ResultadoV]
                var m = Regex.Match(key, @"^ImpactosEspeciales\[(.+?)\]\[(.+?)\]\[(.+?)\]$");
                if (!m.Success) continue;

                if (!int.TryParse(Request.Form[key], out var cantidad) || cantidad <= 0)
                    continue;

                try
                {
                    if (!Enum.TryParse<TipoLado>(m.Groups[1].Value, true, out var lado)) continue;
                    if (!Enum.TryParse<TipoAcciones>(m.Groups[2].Value, true, out var accion)) continue;

                    var resultadoNombre = NormalizarResultado(lado, m.Groups[3].Value);
                    if (!Enum.TryParse<ResultadoAtaque>(resultadoNombre, true, out var resultado)) continue;

                    _context.EstadisticaAtaque.Add(new EstadisticaAtaque
                    {
                        JugadorId = JugadorId,
                        PartidoId = PartidoId,
                        Lado = lado,
                        Accion = accion,
                        Resultado = resultado, // guardamos en variante V
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
            TempData["Mensaje"] = "Ataques actualizados.";
            return RedirectToPage("/Estadisticas/Resumen", new { partidoId = PartidoId });
        }

        // ========= helpers =========

        private static IQueryable<EstadisticaAtaque> FiltrarPorScope(
            IQueryable<EstadisticaAtaque> q, ScopeEstadistica scope, int? setNumero)
        {
            if (scope == ScopeEstadistica.PartidoCompleto)
                return q.Where(e => e.Scope == ScopeEstadistica.PartidoCompleto);

            return q.Where(e =>
                e.Scope == ScopeEstadistica.PartidoCompleto ||
                (e.Scope == ScopeEstadistica.Cierre && (setNumero == null || e.SetNumero == setNumero)));
        }

        private static bool EsEspecial(TipoAcciones a) =>
               a == TipoAcciones.Varios
            || a == TipoAcciones.PorAtras
            || a == TipoAcciones.Atq2daA1
            || a == TipoAcciones.Atq2daA6
            || a == TipoAcciones.Atq2daA5;

        private static ResultadoAtaque NormalizarASimpleV(ResultadoAtaque r) => r switch
        {
            ResultadoAtaque.DoblePositivoE => ResultadoAtaque.DoblePositivoV,
            ResultadoAtaque.PositivoE => ResultadoAtaque.PositivoV,
            ResultadoAtaque.NegativoE => ResultadoAtaque.NegativoV,
            ResultadoAtaque.DobleNegativoE => ResultadoAtaque.DobleNegativoV,
            _ => r
        };

        private static Dictionary<TipoLado, List<TipoAcciones>> BuildAccionesPorLado(Jugador j)
        {
            var res = new Dictionary<TipoLado, List<TipoAcciones>>();

            bool esRol4 = j.RolPrincipal == RolJugador.Rol4;

            // ===== Base de acciones normales (con V/E) =====
            var baseNormales = new List<TipoAcciones>
    {
        TipoAcciones.Atq1, TipoAcciones.Atq2, TipoAcciones.Atq3,
        TipoAcciones.Atq4, TipoAcciones.Atq5, TipoAcciones.Atq6,
        TipoAcciones.Atq7, TipoAcciones.Atq8, TipoAcciones.Atq9
    };

            // ===== Acciones ESPECIALES (sin V/E) =====
            var especiales = new[]
            {
        TipoAcciones.Atq2daA1, TipoAcciones.Atq2daA6, TipoAcciones.Atq2daA5,
        TipoAcciones.Varios,   TipoAcciones.PorAtras
    };

            // ==========================
            //  TOQUES POR LADO / ROL
            //  (MISMO MAPEADO QUE CargarAtaques)
            // ==========================

            // --- Rol 4: lado bueno / atrás ---
            var tlBueno_R4 = new[] { TipoAcciones.Tl1, TipoAcciones.Tl9, TipoAcciones.Tl2 }; // 1-9-2
            var tdBueno_R4 = new[]
            {
        TipoAcciones.Td3, TipoAcciones.Td4, TipoAcciones.Td5,
        TipoAcciones.Td6, TipoAcciones.Td7, TipoAcciones.Td8
    };

            var tlAtras_R4 = new[] { TipoAcciones.Tl5, TipoAcciones.Tl7, TipoAcciones.Tl4 }; // 5-7-4
            var tdAtras_R4 = new[]
            {
        TipoAcciones.Td1, TipoAcciones.Td2, TipoAcciones.Td3,
        TipoAcciones.Td6, TipoAcciones.Td8, TipoAcciones.Td9
    };

            // --- Rol 2: se invierte Bueno <-> Atrás ---
            var tlBueno_R2 = tlAtras_R4;
            var tdBueno_R2 = tdAtras_R4;

            var tlAtras_R2 = tlBueno_R4;
            var tdAtras_R2 = tdBueno_R4;

            // --- LADO MEDIO parametrizado por rol ---
            // Rol 4: TL = 1,9,2 ; TD = 3,4,5,6,7,8
            var tlMedio_R4 = new[]
            {
        TipoAcciones.Tl1, TipoAcciones.Tl9, TipoAcciones.Tl2
    };
            var tdMedio_R4 = new[]
            {
        TipoAcciones.Td3, TipoAcciones.Td4, TipoAcciones.Td5,
        TipoAcciones.Td6, TipoAcciones.Td7, TipoAcciones.Td8
    };

            // Rol 2: TL = 5,7,4 ; TD = 1,2,3,6,8,9
            var tlMedio_R2 = new[]
            {
        TipoAcciones.Tl5, TipoAcciones.Tl7, TipoAcciones.Tl4
    };
            var tdMedio_R2 = new[]
            {
        TipoAcciones.Td1, TipoAcciones.Td2, TipoAcciones.Td3,
        TipoAcciones.Td6, TipoAcciones.Td8, TipoAcciones.Td9
    };

            // ==========================
            //   Construcción por lado
            // ==========================
            foreach (TipoLado lado in Enum.GetValues(typeof(TipoLado)))
            {
                // Empezamos por las normales (Atq1..Atq9)
                var acciones = new List<TipoAcciones>(baseNormales);

                if (lado == TipoLado.Bueno)
                {
                    if (esRol4)
                        acciones.AddRange(tlBueno_R4.Concat(tdBueno_R4));
                    else
                        acciones.AddRange(tlBueno_R2.Concat(tdBueno_R2));
                }
                else if (lado == TipoLado.Medio)
                {
                    if (esRol4)
                        acciones.AddRange(tlMedio_R4.Concat(tdMedio_R4));
                    else
                        acciones.AddRange(tlMedio_R2.Concat(tdMedio_R2));
                }
                else // Atrás
                {
                    if (esRol4)
                        acciones.AddRange(tlAtras_R4.Concat(tdAtras_R4));
                    else
                        acciones.AddRange(tlAtras_R2.Concat(tdAtras_R2));
                }

                // SIEMPRE al final las ESPECIALES (sin V/E)
                acciones.AddRange(especiales);

                res[lado] = acciones;
            }

            return res;
        }

    }
}
