using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SandStats.Data;
using SandStats.Models;
using SandStats.Models.SandStats.Models; // EstadisticaRecepcion + enums
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SandStats.Pages.Estadisticas
{
    public class ReporteDuplaModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public ReporteDuplaModel(ApplicationDbContext db) => _db = db;

        // ===== Parámetros (GET) =====
        [BindProperty(SupportsGet = true)] public int DuplaId { get; set; }
        [BindProperty(SupportsGet = true)] public int? PartidoId { get; set; }
        [BindProperty(SupportsGet = true)] public List<int>? PartidoIds { get; set; }
        [BindProperty(SupportsGet = true)] public bool IncluirCierre { get; set; }
        [BindProperty(SupportsGet = true)] public int? SetNumero { get; set; }

        // ===== Cabecera =====
        public Dupla Dupla { get; set; } = default!;
        public List<Partido> PartidosSeleccionados { get; set; } = new();
        public string? Subtitulo { get; set; }

        // ===== Reportes por jugador (Recepción + Ataque + K2) =====
        public RecepcionPlayerReport RepJ1 { get; set; } = default!;
        public RecepcionPlayerReport RepJ2 { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            if (DuplaId == 0) return RedirectToPage("/Duplas/Index");

            Dupla = await _db.Duplas
                .Include(d => d.Jugador1).Include(d => d.Jugador2)
                .FirstOrDefaultAsync(d => d.Id == DuplaId)
                ?? throw new Exception("Dupla no encontrada");

            // ----- Partidos a considerar -----
            var pids = new List<int>();
            if (PartidoId.HasValue) pids.Add(PartidoId.Value);
            else if (PartidoIds != null && PartidoIds.Count > 0) pids.AddRange(PartidoIds.Distinct());

            PartidosSeleccionados = await _db.Partidos
                .Include(p => p.Dupla1).Include(p => p.Dupla2)
                .Where(p => pids.Contains(p.Id))
                .OrderBy(p => p.Fecha)
                .ToListAsync();

            var cant = PartidosSeleccionados.Count;
            Subtitulo = cant == 0 ? "Sin partidos seleccionados"
                      : cant == 1 ? "Incluye 1 partido"
                      : $"Incluye {cant} partidos";

            // Base vacía si no hay partidos
            if (cant == 0)
            {
                RepJ1 = RecepcionPlayerReport.Empty(Dupla.Jugador1!);
                RepJ2 = RecepcionPlayerReport.Empty(Dupla.Jugador2!);
                return Page();
            }

            // ----- Query base Recepción (ambos jugadores) -----
            var j1Id = Dupla.Jugador1Id;
            var j2Id = Dupla.Jugador2Id;

            var qRecBase = _db.EstadisticaRecepcion.AsNoTracking()
                .Where(e => pids.Contains(e.PartidoId) && (e.JugadorId == j1Id || e.JugadorId == j2Id));

            qRecBase = FiltrarPorScope(qRecBase, IncluirCierre, SetNumero);

            // Totales equipo (recepción) para % sobre equipo
            var totalEquipoRec = await qRecBase.CountAsync();

            // Reporte Recepción por jugador
            RepJ1 = await ConstruirReporteRecepcion(Dupla.Jugador1!, qRecBase.Where(e => e.JugadorId == j1Id), totalEquipoRec);
            RepJ2 = await ConstruirReporteRecepcion(Dupla.Jugador2!, qRecBase.Where(e => e.JugadorId == j2Id), totalEquipoRec);

            // ----- Query base ATAQUE (ambos jugadores) -----
            var qAtkBase = _db.EstadisticaAtaque.AsNoTracking()
                .Where(e => pids.Contains(e.PartidoId) && (e.JugadorId == j1Id || e.JugadorId == j2Id));

            qAtkBase = FiltrarPorScope(qAtkBase, IncluirCierre, SetNumero);

            // Totales equipo (ataque) = suma de Cantidad
            var totalEquipoAtk = await qAtkBase.SumAsync(e => (int?)e.Cantidad) ?? 0;

            // Armar Ataque dentro del reporte de cada jugador
            RepJ1.Ataque = await ConstruirReporteAtaque(Dupla.Jugador1!, qAtkBase.Where(e => e.JugadorId == j1Id), totalEquipoAtk);
            RepJ2.Ataque = await ConstruirReporteAtaque(Dupla.Jugador2!, qAtkBase.Where(e => e.JugadorId == j2Id), totalEquipoAtk);

            // Si querés fijar manualmente el rol visual:
            RepJ1.Ataque.Rol = 4; // jugador de rol por 4
            RepJ2.Ataque.Rol = 2; // jugador de rol por 2

            // ----- K2 (cuadro único por jugador) -----
            var pid = PartidoId ?? PartidosSeleccionados?.FirstOrDefault()?.Id;

            // Referencia al Partido (necesaria para calcular SetsJugados)
            Partido? partidoRef = null;
            if (pid.HasValue)
                partidoRef = PartidosSeleccionados.FirstOrDefault(p => p.Id == pid.Value)
                             ?? await _db.Partidos.FindAsync(pid.Value);

            // J1
            RepJ1.K2 = await CargarK2JugadorAsync(RepJ1.Jugador.Id, pid);
            RepJ1.K2.Partido = partidoRef;          // 👈 para SetsJugados
            RepJ1.Ataque.K2 = RepJ1.K2;             // 👈 Ataque ahora “ve” K2

            // J2
            RepJ2.K2 = await CargarK2JugadorAsync(RepJ2.Jugador.Id, pid);
            RepJ2.K2.Partido = partidoRef;          // 👈 para SetsJugados
            RepJ2.Ataque.K2 = RepJ2.K2;             // 👈 Ataque ahora “ve” K2



            return Page();
        }

        // ========= Filtros Scope =========
        private static IQueryable<EstadisticaRecepcion> FiltrarPorScope(
            IQueryable<EstadisticaRecepcion> q, bool incluirCierre, int? setNumero)
        {
            if (!incluirCierre)
                return q.Where(e => e.Scope == ScopeEstadistica.PartidoCompleto);

            return q.Where(e =>
                e.Scope == ScopeEstadistica.PartidoCompleto ||
                (e.Scope == ScopeEstadistica.Cierre && (setNumero == null || e.SetNumero == setNumero)));
        }

        private static IQueryable<EstadisticaAtaque> FiltrarPorScope(
            IQueryable<EstadisticaAtaque> q, bool incluirCierre, int? setNumero)
        {
            if (!incluirCierre)
                return q.Where(e => e.Scope == ScopeEstadistica.PartidoCompleto);

            return q.Where(e =>
                e.Scope == ScopeEstadistica.PartidoCompleto ||
                (e.Scope == ScopeEstadistica.Cierre && (setNumero == null || e.SetNumero == setNumero)));
        }

        // ================= RECEPCIÓN =================

        private async Task<RecepcionPlayerReport> ConstruirReporteRecepcion(
            Jugador jugador,
            IQueryable<EstadisticaRecepcion> qJugador,
            int totalEquipo)
        {
            // Totales generales jugador
            var countsAll = await ContarResultadosRec(qJugador);
            var totalJugador = countsAll.Total;

            // Por TipoRecepcion (suma Flot+Pot)
            var porTipo_All = await ContarPorTipoRecepcion(qJugador);

            // Por sectores (suma Flot+Pot)
            var sectores_All = await ContarPorSector(qJugador, totalJugador);

            // Flotados
            var qF = qJugador.Where(e => e.TipoSaque == TipoSaque.Flotado);
            var flotCounts = await ContarResultadosRec(qF);
            var porTipo_F = await ContarPorTipoRecepcion(qF);
            var sectores_F = await ContarPorSector(qF, flotCounts.Total);

            // Potencia
            var qP = qJugador.Where(e => e.TipoSaque == TipoSaque.Potencia);
            var potCounts = await ContarResultadosRec(qP);
            var porTipo_P = await ContarPorTipoRecepcion(qP);
            var sectores_P = await ContarPorSector(qP, potCounts.Total);

            return new RecepcionPlayerReport
            {
                Jugador = jugador,
                General = new RecepcionResumen
                {
                    Totales = countsAll,
                    PctSobreEquipo = totalEquipo == 0 ? 0m : (decimal)totalJugador / totalEquipo,
                    PorTipo = porTipo_All,
                    Sectores = sectores_All,
                    TotalEquipo = totalEquipo

                },
                Flotados = new RecepcionSeccionTipoSaque
                {
                    Totales = flotCounts,
                    PctSobreJugador = totalJugador == 0 ? 0m : (decimal)flotCounts.Total / totalJugador,
                    PorTipo = porTipo_F,
                    Sectores = sectores_F,
                },
                Potencia = new RecepcionSeccionTipoSaque
                {
                    Totales = potCounts,
                    PctSobreJugador = totalJugador == 0 ? 0m : (decimal)potCounts.Total / totalJugador,
                    PorTipo = porTipo_P,
                    Sectores = sectores_P
                }
            };
        }

        private static async Task<RecCounts> ContarResultadosRec(IQueryable<EstadisticaRecepcion> q)
        {
            var groups = await q.GroupBy(e => e.ResultadoRecepcion)
                                .Select(g => new { g.Key, C = g.Count() })
                                .ToListAsync();

            int dp = groups.FirstOrDefault(x => x.Key == ResultadoRecepcion.doblePositivo)?.C ?? 0;
            int p = groups.FirstOrDefault(x => x.Key == ResultadoRecepcion.positivo)?.C ?? 0;
            int n = groups.FirstOrDefault(x => x.Key == ResultadoRecepcion.negativo)?.C ?? 0;
            int e = groups.FirstOrDefault(x => x.Key == ResultadoRecepcion.dobleNegativo)?.C ?? 0;

            return new RecCounts(dp, p, n, e);
        }

        private static async Task<Dictionary<TipoRecepcion, RecCounts>> ContarPorTipoRecepcion(IQueryable<EstadisticaRecepcion> q)
        {
            var raw = await q.GroupBy(e => new { e.TipoRecepcion, e.ResultadoRecepcion })
                             .Select(g => new { g.Key.TipoRecepcion, g.Key.ResultadoRecepcion, C = g.Count() })
                             .ToListAsync();

            var dict = Enum.GetValues<TipoRecepcion>()
                           .ToDictionary(t => t, t => new RecCounts(0, 0, 0, 0));

            foreach (var r in raw)
            {
                var c = dict[r.TipoRecepcion];
                switch (r.ResultadoRecepcion)
                {
                    case ResultadoRecepcion.doblePositivo: c.DP += r.C; break;
                    case ResultadoRecepcion.positivo: c.P += r.C; break;
                    case ResultadoRecepcion.negativo: c.N += r.C; break;
                    case ResultadoRecepcion.dobleNegativo: c.E += r.C; break;
                }
            }
            return dict;
        }

        private static async Task<List<RecSector>> ContarPorSector(IQueryable<EstadisticaRecepcion> q, int totalDenominador)
        {
            var raw = await q.GroupBy(e => new { e.ZonaRecepcion, e.TipoRecepcion, e.ResultadoRecepcion })
                             .Select(g => new
                             {
                                 g.Key.ZonaRecepcion,
                                 g.Key.TipoRecepcion,
                                 g.Key.ResultadoRecepcion,
                                 C = g.Count()
                             })
                             .ToListAsync();

            var zonas = new[] { ZonaSaque.Zona1, ZonaSaque.Zona6, ZonaSaque.Zona5 };

            var lista = new List<RecSector>();
            foreach (var z in zonas)
            {
                var porTipo = Enum.GetValues<TipoRecepcion>()
                                  .ToDictionary(t => t, t => new RecCounts(0, 0, 0, 0));

                foreach (var r in raw.Where(x => x.ZonaRecepcion == z))
                {
                    var c = porTipo[r.TipoRecepcion];
                    switch (r.ResultadoRecepcion)
                    {
                        case ResultadoRecepcion.doblePositivo: c.DP += r.C; break;
                        case ResultadoRecepcion.positivo: c.P += r.C; break;
                        case ResultadoRecepcion.negativo: c.N += r.C; break;
                        case ResultadoRecepcion.dobleNegativo: c.E += r.C; break;
                    }
                }

                var totalZ = porTipo.Values.Sum(v => v.Total);
                lista.Add(new RecSector
                {
                    Zona = z,
                    Total = totalZ,
                    PctSobreJugador = totalDenominador == 0 ? 0m : (decimal)totalZ / totalDenominador,
                    PorTipo = porTipo
                });
            }
            return lista;
        }

        // ================= ATAQUE =================

        private async Task<AtaquePlayerReport> ConstruirReporteAtaque(
    Jugador jugador,
    IQueryable<EstadisticaAtaque> qJugador,
    int totalEquipo)
        {
            // Totales jugador (suma de Cantidad) – excluye PorAtras en el método
            var countsAll = await ContarResultadosAtk(qJugador);
            var totalJugador = countsAll.Total;

            // Por acción
            var porAccion = await ContarPorAccion(qJugador);

            // Por familias (Atq, Tl, Td, Atq2da, Varios, PorAtras)
            var familias = AgruparFamilias(porAccion);

            // Denominador: K1 SIN 2da para TODO el jugador (Atq/Tl/Td sin Atq2da, y sin PorAtras)
            var totalK1Sin2daJugador = porAccion
                .Where(kv => kv.Key != TipoAcciones.PorAtras     // espejo, no suma
                && EsK1(kv.Key)                        // Atq, Tl, Td, Varios
                && !EsAtq2da(kv.Key))                  // sin 2da
                .Sum(kv => kv.Value.Total);


            // Por lado (Bueno/Medio/Atrás)
            var lados = await ContarPorLado(qJugador, totalJugador,totalK1Sin2daJugador);

            // ==== K1 / K2 (Excel-like) =========================================
            // K2 = Atq2da + Varios  (NO contamos PorAtras porque es espejo)
            AtkCounts FamOrEmpty(string key) =>
                familias.TryGetValue(key, out var v) ? v : new AtkCounts();

            var atq2da = FamOrEmpty("Atq2da");
            var varios = FamOrEmpty("Varios");
            // PorAtras disponible si lo querés mostrar aparte, pero NO suma en K2/Totales
            var porAtras = FamOrEmpty("PorAtras");

            var k2Counts = new AtkCounts(
                atq2da.DP + varios.DP,
                atq2da.P + varios.P,
                atq2da.N + varios.N,
                atq2da.E + varios.E
            );
            var k2Total = k2Counts.Total;

            // K1 total = TotalJugador - K2
            var k1Total = Math.Max(0, totalJugador - k2Total);

            // K1 por “segunda” y “sin segunda”
            var k1De2da = atq2da.Total;             // solo familia Atq2da
            var k1Sin2da = Math.Max(0, k1Total - k1De2da);

            // Para Efectividad K1, restamos del total global los impactos que son K2
            int k1DP = countsAll.DP - k2Counts.DP;
            int k1P = countsAll.P - k2Counts.P;
            int k1N = countsAll.N - k2Counts.N;
            int k1E = countsAll.E - k2Counts.E;

            decimal efectK1 = k1Total > 0 ? (k1DP + k1P) / (decimal)k1Total : 0m;
            decimal efectAtk = countsAll.Efect;

            decimal pctK1 = totalJugador > 0 ? k1Total / (decimal)totalJugador : 0m;
            decimal pctNoK1 = 1m - pctK1;

            // ===================================================================

            return new AtaquePlayerReport
            {
                Jugador = jugador,

                Totales = countsAll,
                PctSobreEquipo = totalEquipo == 0 ? 0m : (decimal)totalJugador / totalEquipo,
                PorAccion = porAccion
                    .Where(kv => kv.Value.Total > 0)
                    .OrderByDescending(kv => kv.Value.Total)
                    .ToDictionary(kv => kv.Key, kv => kv.Value),
                Familias = familias,
                Lados = lados,

                // === Métricas K1/K2 para el cuadro principal ===
                TotalK1 = k1Total,
                TotalK1Sin2da = k1Sin2da,
                TotalK1De2da = k1De2da,
                PctK1 = pctK1,
                PctNoK1 = pctNoK1,
                EfectividadK1 = efectK1,
                EfectividadAtaque = efectAtk
            };
        }
        private static bool EsK1(TipoAcciones a)
        {
            var s = a.ToString();
            return s.StartsWith("Atq", StringComparison.OrdinalIgnoreCase)
                || s.StartsWith("Tl", StringComparison.OrdinalIgnoreCase)
                || s.StartsWith("Td", StringComparison.OrdinalIgnoreCase)
                || s.Equals("Varios", StringComparison.OrdinalIgnoreCase); // ✅ ahora suma en K1

        }

        private static bool EsAtq2da(TipoAcciones a)
            => a.ToString().StartsWith("Atq2da", StringComparison.OrdinalIgnoreCase);

        private static async Task<AtkCounts> ContarResultadosAtk(IQueryable<EstadisticaAtaque> q)
        {
            // 🚫 Excluir 'PorAtras' de los totales generales (es un espejo)
            q = q.Where(e => e.Accion != TipoAcciones.PorAtras);

            var raw = await q.GroupBy(e => e.Resultado)
                             .Select(g => new { g.Key, S = g.Sum(x => x.Cantidad) })
                             .ToListAsync();

            int dp = 0, p = 0, n = 0, e0 = 0;

            foreach (var r in raw)
            {
                switch (r.Key)
                {
                    case ResultadoAtaque.DoblePositivoV:
                    case ResultadoAtaque.DoblePositivoE: dp += r.S; break;
                    case ResultadoAtaque.PositivoV:
                    case ResultadoAtaque.PositivoE: p += r.S; break;
                    case ResultadoAtaque.NegativoV:
                    case ResultadoAtaque.NegativoE: n += r.S; break;
                    case ResultadoAtaque.DobleNegativoV:
                    case ResultadoAtaque.DobleNegativoE: e0 += r.S; break;
                }
            }
            return new AtkCounts(dp, p, n, e0);
        }


        private static async Task<Dictionary<TipoAcciones, AtkCounts>> ContarPorAccion(IQueryable<EstadisticaAtaque> q)
        {
            var raw = await q.GroupBy(e => new { e.Accion, e.Resultado })
                             .Select(g => new { g.Key.Accion, g.Key.Resultado, S = g.Sum(x => x.Cantidad) })
                             .ToListAsync();

            var dict = Enum.GetValues<TipoAcciones>()
                           .ToDictionary(a => a, a => new AtkCounts(0, 0, 0, 0));

            foreach (var r in raw)
            {
                var c = dict[r.Accion];
                switch (r.Resultado)
                {
                    case ResultadoAtaque.DoblePositivoV:
                    case ResultadoAtaque.DoblePositivoE: c.DP += r.S; break;

                    case ResultadoAtaque.PositivoV:
                    case ResultadoAtaque.PositivoE: c.P += r.S; break;

                    case ResultadoAtaque.NegativoV:
                    case ResultadoAtaque.NegativoE: c.N += r.S; break;

                    case ResultadoAtaque.DobleNegativoV:
                    case ResultadoAtaque.DobleNegativoE: c.E += r.S; break;
                }
            }
            return dict;
        }

        private static Dictionary<string, AtkCounts> AgruparFamilias(Dictionary<TipoAcciones, AtkCounts> porAccion)
        {
            var result = new Dictionary<string, AtkCounts>
            {
                ["Atq"] = new AtkCounts(),
                ["Tl"] = new AtkCounts(),
                ["Td"] = new AtkCounts(),
                ["Atq2da"] = new AtkCounts(),
                ["Varios"] = new AtkCounts(),
                ["PorAtras"] = new AtkCounts()
            };

            foreach (var kv in porAccion)
            {
                var k = kv.Key.ToString();

                string fam = k.StartsWith("Atq2da", StringComparison.OrdinalIgnoreCase) ? "Atq2da"
                            : k.StartsWith("Atq", StringComparison.OrdinalIgnoreCase) ? "Atq"
                            : k.StartsWith("Tl", StringComparison.OrdinalIgnoreCase) ? "Tl"
                            : k.StartsWith("Td", StringComparison.OrdinalIgnoreCase) ? "Td"
                            : k.Equals("Varios", StringComparison.OrdinalIgnoreCase) ? "Varios"
                            : k.Equals("PorAtras", StringComparison.OrdinalIgnoreCase) ? "PorAtras"
                            : "Varios";

                var dst = result[fam];
                dst.DP += kv.Value.DP;
                dst.P += kv.Value.P;
                dst.N += kv.Value.N;
                dst.E += kv.Value.E;
            }

            // remover familias con Total=0
            return result.Where(x => x.Value.Total > 0)
                         .ToDictionary(x => x.Key, x => x.Value);
        }

        private static bool EsV(ResultadoAtaque r) =>
                    r == ResultadoAtaque.DoblePositivoV || r == ResultadoAtaque.PositivoV ||
                    r == ResultadoAtaque.NegativoV || r == ResultadoAtaque.DobleNegativoV;

        private static bool EsE(ResultadoAtaque r) =>
                    r == ResultadoAtaque.DoblePositivoE || r == ResultadoAtaque.PositivoE ||
                    r == ResultadoAtaque.NegativoE || r == ResultadoAtaque.DobleNegativoE;

        private static async Task<List<AtaqueLado>> ContarPorLado(
    IQueryable<EstadisticaAtaque> q,
    int totalJugador,
    int totalK1Sin2daJugador)   // 👈 denominador correcto para % por lado
        {
            var raw = await q.GroupBy(e => new { e.Lado, e.Accion, e.Resultado })
                             .Select(g => new { g.Key.Lado, g.Key.Accion, g.Key.Resultado, S = g.Sum(x => x.Cantidad) })
                             .ToListAsync();

            var lados = new[] { TipoLado.Bueno, TipoLado.Medio, TipoLado.Atras };
            var lista = new List<AtaqueLado>();

            foreach (var lado in lados)
            {
                var porAccion = Enum.GetValues<TipoAcciones>().ToDictionary(a => a, a => new AtkCounts());

                // 👇 NUEVO: contadores V/E por ACCIÓN (además del total por lado)
                var varillaPorAccion = Enum.GetValues<TipoAcciones>().ToDictionary(a => a, a => 0);
                var entreLineaPorAccion = Enum.GetValues<TipoAcciones>().ToDictionary(a => a, a => 0);

                var tot = new AtkCounts();
                int varT = 0, linT = 0;

                int sum2daEsteLado = 0;
                int k1Sin2daEsteLado = 0;

                foreach (var r in raw.Where(x => x.Lado == lado))
                {
                    // espejo fuera de totales
                    if (r.Accion == TipoAcciones.PorAtras) continue;

                    var c = porAccion[r.Accion];
                    switch (r.Resultado)
                    {
                        // VARILLA
                        case ResultadoAtaque.DoblePositivoV: c.DP += r.S; tot.DP += r.S; varT += r.S; varillaPorAccion[r.Accion] += r.S; break;
                        case ResultadoAtaque.PositivoV: c.P += r.S; tot.P += r.S; varT += r.S; varillaPorAccion[r.Accion] += r.S; break;
                        case ResultadoAtaque.NegativoV: c.N += r.S; tot.N += r.S; varT += r.S; varillaPorAccion[r.Accion] += r.S; break;
                        case ResultadoAtaque.DobleNegativoV: c.E += r.S; tot.E += r.S; varT += r.S; varillaPorAccion[r.Accion] += r.S; break;

                        // ENTRE LÍNEA
                        case ResultadoAtaque.DoblePositivoE: c.DP += r.S; tot.DP += r.S; linT += r.S; entreLineaPorAccion[r.Accion] += r.S; break;
                        case ResultadoAtaque.PositivoE: c.P += r.S; tot.P += r.S; linT += r.S; entreLineaPorAccion[r.Accion] += r.S; break;
                        case ResultadoAtaque.NegativoE: c.N += r.S; tot.N += r.S; linT += r.S; entreLineaPorAccion[r.Accion] += r.S; break;
                        case ResultadoAtaque.DobleNegativoE: c.E += r.S; tot.E += r.S; linT += r.S; entreLineaPorAccion[r.Accion] += r.S; break;
                    }

                    if (r.Accion == TipoAcciones.Atq2daA1 ||
                        r.Accion == TipoAcciones.Atq2daA6 ||
                        r.Accion == TipoAcciones.Atq2daA5)
                    {
                        sum2daEsteLado += r.S;
                    }

                    // numerador de “K1 sin 2da” por lado
                    if (EsK1(r.Accion) && !EsAtq2da(r.Accion))
                        k1Sin2daEsteLado += r.S;
                }

                if (tot.Total == 0) continue;

                var dictConDatos = porAccion.Where(kv => kv.Value.Total > 0)
                                            .ToDictionary(kv => kv.Key, kv => kv.Value);

                // 👇 al crear cada fila (AtaqueAccion), rellenamos los totales V/E por acción
                var accionesVm = dictConDatos.Select(kv => new AtaqueAccion
                {
                    Accion = kv.Key,
                    C = kv.Value,
                    TotalVarilla = (lado == TipoLado.Medio) ? 0 : varillaPorAccion[kv.Key],
                    TotalEntreLinea = (lado == TipoLado.Medio) ? 0 : entreLineaPorAccion[kv.Key]
                }).ToList();

                lista.Add(new AtaqueLado
                {
                    Lado = lado,
                    Totales = tot,
                    Total = tot.Total,
                    PctSobreJugador = totalJugador == 0 ? 0m : (decimal)tot.Total / totalJugador,
                    Acciones = accionesVm,

                    // totales V/E por LADO (ya los tenías)
                    TotalVarilla = varT,
                    TotalEntreLinea = linT,

                    Pct2daSobreTotal = totalJugador == 0 ? 0m : (decimal)sum2daEsteLado / totalJugador,

                    PctK1Sin2daSobreJugador = totalK1Sin2daJugador == 0
                        ? 0m
                        : (decimal)k1Sin2daEsteLado / totalK1Sin2daJugador
                });
            }

            return lista;
        }




        // ================= K2 (único cuadro) =================

        private async Task<K2Report> CargarK2JugadorAsync(int jugadorId, int? partidoId = null)
        {
            var q = _db.EstadisticaK2.AsNoTracking().Where(e => e.JugadorId == jugadorId);

            if (partidoId.HasValue)
                q = q.Where(e => e.PartidoId == partidoId.Value);
            else if (PartidoId.HasValue)
                q = q.Where(e => e.PartidoId == PartidoId.Value);
            else if (PartidosSeleccionados?.Any() == true)
                q = q.Where(e => PartidosSeleccionados.Select(p => p.Id).Contains(e.PartidoId));

            // ⚠️ A veces los datos quedan en Cierre, otras en PartidoCompleto.
            // Para no “comernos” datos, aceptamos ambos:
            q = q.Where(e => e.Scope == ScopeEstadistica.PartidoCompleto
                          || e.Scope == ScopeEstadistica.Cierre);

            var raw = await q
                .GroupBy(e => new { e.Fuente, e.Resultado })
                .Select(g => new { g.Key.Fuente, g.Key.Resultado, Cant = g.Sum(x => x.Cantidad) })
                .ToListAsync();

            K2Counts Build(FuenteK2 f) => new K2Counts
            {
                DP = raw.Where(x => x.Fuente == f && x.Resultado == ResultadoK2.DoblePositivo).Sum(x => x.Cant),
                P = raw.Where(x => x.Fuente == f && x.Resultado == ResultadoK2.Positivo).Sum(x => x.Cant),
                N = raw.Where(x => x.Fuente == f && x.Resultado == ResultadoK2.Negativo).Sum(x => x.Cant),
                DN = raw.Where(x => x.Fuente == f && x.Resultado == ResultadoK2.DobleNegativo).Sum(x => x.Cant),
            };

            // 🔎 fallback: si no hay registros con Fuente=PuntosJugados, calculo totalK2 sumando todas las fuentes
            int totalK2Fallback = raw.Sum(x => x.Cant);

            var any = await q.FirstOrDefaultAsync(x => x.Agregados.HasValue || x.ErroresVarios.HasValue);

            var rep = new K2Report
            {
                PuntosJugados = Build(FuenteK2.PuntosJugados),
                SaquesFlotado = Build(FuenteK2.SaqueFlotado),
                SaquesPotencia = Build(FuenteK2.SaquePotencia),
                BloqueoA1 = Build(FuenteK2.BloqueoAtqA1),
                BloqueoA6 = Build(FuenteK2.BloqueoAtqA6),
                BloqueoA5 = Build(FuenteK2.BloqueoAtqA5),
                ErroresVarios = any?.ErroresVarios ?? 0,
                Agregados = any?.Agregados ?? 0
            };

            // si PuntosJugados.Total quedó en 0 pero hay datos, uso el fallback
            if (rep.PuntosJugados.Total == 0 && totalK2Fallback > 0)
            {
                rep.PuntosJugados.DP = 0;
                rep.PuntosJugados.P = totalK2Fallback; // o repartilo si tenés el detalle; sirve para contar total
            }

            return rep;
        }




    }

    // ==================== ViewModels RECEPCIÓN ====================

    public class RecCounts
    {
        public int DP { get; set; }
        public int P { get; set; }
        public int N { get; set; }
        public int E { get; set; }

        public int Total => DP + P + N + E;
        public decimal Efect => Total == 0 ? 0m : (DP + P) / (decimal)Total;

        public decimal PctDP => Total == 0 ? 0m : DP / (decimal)Total;
        public decimal PctP => Total == 0 ? 0m : P / (decimal)Total;
        public decimal PctN => Total == 0 ? 0m : N / (decimal)Total;
        public decimal PctE => Total == 0 ? 0m : E / (decimal)Total;

        public RecCounts() { }
        public RecCounts(int dp, int p, int n, int e) { DP = dp; P = p; N = n; E = e; }
    }

    public class RecSector
    {
        public ZonaSaque Zona { get; set; }
        public int Total { get; set; }
        public decimal PctSobreJugador { get; set; }
        public Dictionary<TipoRecepcion, RecCounts> PorTipo { get; set; } = new();
    }

    public class RecepcionResumen
    {
        public RecCounts Totales { get; set; } = new(0, 0, 0, 0);
        public decimal PctSobreEquipo { get; set; }
        public Dictionary<TipoRecepcion, RecCounts> PorTipo { get; set; } = new();
        public List<RecSector> Sectores { get; set; } = new();
        public int TotalEquipo { get; set; }   // <<< agregar esta línea

    }

    public class RecepcionSeccionTipoSaque
    {
        public RecCounts Totales { get; set; } = new(0, 0, 0, 0);
        public decimal PctSobreJugador { get; set; }
        public Dictionary<TipoRecepcion, RecCounts> PorTipo { get; set; } = new();
        public List<RecSector> Sectores { get; set; } = new();
    }

    // ==================== ViewModels ATAQUE ====================

    public class AtkCounts
    {
        public int DP { get; set; }
        public int P { get; set; }
        public int N { get; set; }
        public int E { get; set; }

        public int Total => DP + P + N + E;
        public decimal Efect => Total == 0 ? 0m : (DP + P) / (decimal)Total;

        public decimal PctDP => Total == 0 ? 0m : DP / (decimal)Total;
        public decimal PctP => Total == 0 ? 0m : P / (decimal)Total;
        public decimal PctN => Total == 0 ? 0m : N / (decimal)Total;
        public decimal PctE => Total == 0 ? 0m : E / (decimal)Total;

        public AtkCounts() { }
        public AtkCounts(int dp, int p, int n, int e) { DP = dp; P = p; N = n; E = e; }
    }

    public class AtaqueAccion
    {
        public TipoAcciones Accion { get; set; }
        public AtkCounts C { get; set; } = new();
        public decimal PctDentroDelLado { get; set; }
        // NUEVO: desglose V/E por acción (solo aplica para Bueno/Atrás)
        public int TotalVarilla { get; set; }      // suma de ...V
        public int TotalEntreLinea { get; set; }   // suma de ...E
    }

    public class AtaqueLado
    {
        public TipoLado Lado { get; set; }
        public AtkCounts Totales { get; set; } = new();  // total del lado
        public decimal PctSobreJugador { get; set; }

        public int Total { get; set; }
        public int TotalVarilla { get; set; }
        public int TotalEntreLinea { get; set; }
        // ✅ NUEVO: % de salidas K1 del lado sobre el K1 SIN 2da del jugador
        public decimal PctK1Sin2daSobreJugador { get; set; }

        public Dictionary<TipoAcciones, AtkCounts> PorAccion { get; set; } = new();
        public List<AtaqueAccion> Acciones { get; set; } = new();
        // <<< NUEVO: % de 2da por lado (Atq2da_lado / total ataques jugador)
        public decimal Pct2daSobreTotal { get; set; }

    }

    public class AtaqueResumen
    {
        public AtkCounts Totales { get; set; } = new(0, 0, 0, 0);
        public decimal PctSobreEquipo { get; set; }
        public Dictionary<TipoAcciones, AtkCounts> PorAccion { get; set; } = new();
        public Dictionary<string, AtkCounts> Familias { get; set; } = new();
        public List<AtaqueLado> Lados { get; set; } = new();
    }

    /// <summary>
    /// Exposición en raíz + compatibilidad con General.
    /// </summary>
    public class AtaquePlayerReport
    {
        public Jugador Jugador { get; set; } = default!;

        public AtkCounts Totales { get; set; } = new(0, 0, 0, 0);
        public decimal PctSobreEquipo { get; set; }
        public Dictionary<TipoAcciones, AtkCounts> PorAccion { get; set; } = new();
        public Dictionary<string, AtkCounts> Familias { get; set; } = new();
        public List<AtaqueLado> Lados { get; set; } = new();

        public int Rol { get; set; } = 2; // 4 o 2
                                          // ======== NUEVO: métricas K1/K2 para mostrar en el resumen ========
                                          // Total K1 mostrado en el cuadro principal
        public int TotalK1 { get; set; }
        public int TotalK1Sin2da { get; set; }
        public int TotalK1De2da { get; set; }

        public decimal PctK1 { get; set; }
        public decimal PctNoK1 { get; set; }
        public K2Report? K2 { get; set; }

        public decimal EfectividadK1 { get; set; }
        public decimal EfectividadAtaque { get; set; }
        public AtaqueResumen General
        {
            get => new AtaqueResumen
            {
                Totales = Totales,
                PctSobreEquipo = PctSobreEquipo,
                PorAccion = PorAccion,
                Familias = Familias,
                Lados = Lados
            };
            set
            {
                if (value == null) return;
                Totales = value.Totales ?? new AtkCounts();
                PctSobreEquipo = value.PctSobreEquipo;
                PorAccion = value.PorAccion ?? new();
                Familias = value.Familias ?? new();
                Lados = value.Lados ?? new();
            }
        }
    }

    // ==================== ViewModel K2 (único cuadro) ====================

    public class K2Counts
    {
        public int DP { get; set; }   // #
        public int P { get; set; }   // +
        public int N { get; set; }   // -
        public int DN { get; set; }   // =

        public int Total => DP + P + N + DN;

        public decimal Efect => Total > 0 ? (DP + P) / (decimal)Total : 0m; // headline
        public decimal PctDP => Total > 0 ? DP / (decimal)Total : 0m;
        public decimal PctP => Total > 0 ? P / (decimal)Total : 0m;
        public decimal PctN => Total > 0 ? N / (decimal)Total : 0m;
        public decimal PctDN => Total > 0 ? DN / (decimal)Total : 0m;
    }
  
        public class K2Report
        {
            public K2Counts PuntosJugados { get; set; } = new();
            public K2Counts SaquesFlotado { get; set; } = new();
            public K2Counts SaquesPotencia { get; set; } = new();
            public K2Counts BloqueoA1 { get; set; } = new();
            public K2Counts BloqueoA6 { get; set; } = new();
            public K2Counts BloqueoA5 { get; set; } = new();
            // 👇 agregá esta propiedad
            public Partido? Partido { get; set; }

            // ahora podés calcular SetsJugados
            public int SetsJugados => Partido?.SetsJugados ?? 0;
            public decimal EfectHeadline => PuntosJugados.Efect;
            public int ErroresVarios { get; set; }
            public int Agregados { get; set; }
        

    }
    // ==================== Compuesto por jugador (Recep + Atq + K2) ====================

    public class RecepcionPlayerReport
    {
        public Jugador Jugador { get; set; } = default!;
        public RecepcionResumen General { get; set; } = new();
        public RecepcionSeccionTipoSaque Flotados { get; set; } = new();
        public RecepcionSeccionTipoSaque Potencia { get; set; } = new();

        // Ataque integrado debajo de Recepción
        public AtaquePlayerReport Ataque { get; set; } = new();

        // ✅ Debe ser K2Report (no K2Counts)
        public K2Report K2 { get; set; } = new();

        public static RecepcionPlayerReport Empty(Jugador j) => new()
        {
            Jugador = j,
            General = new RecepcionResumen(),
            Flotados = new RecepcionSeccionTipoSaque(),
            Potencia = new RecepcionSeccionTipoSaque(),
            Ataque = new AtaquePlayerReport { Jugador = j },
            K2 = new K2Report()
        };
    }
}
