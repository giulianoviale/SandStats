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

        public MapaAtaqueViewModel? MapaJ1 { get; set; }
        public MapaAtaqueViewModel? MapaJ2 { get; set; } // si lo querés para el otro jugador

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

            // --- ejemplo para J1 ---
            var mapaJ1 = new MapaAtaqueViewModel
            {
                Titulo = "Atq línea",
                PlayerRight = false,
                TotalAtaques = RepJ1.Ataque.Totales.Total,
                Rol = RepJ1.Ataque.Rol,
                Lado = TipoLado.Bueno
            };

            // Mapear acciones y totales V/E globales para el mapa
            mapaJ1.Acciones = BuildMapaAccionesFromLados(RepJ1.Ataque.Lados);

            // Totales V/E del mapa (suma de acciones)
            mapaJ1.TotalVarilla = mapaJ1.Acciones.Sum(x => x.TotalVarilla);
            mapaJ1.TotalEntreLinea = mapaJ1.Acciones.Sum(x => x.TotalEntreLinea);

            this.MapaJ1 = mapaJ1;

            // ----- K2 (cuadro único por jugador) -----
            int? pid = PartidosSeleccionados.Count == 1
                ? PartidosSeleccionados[0].Id
                : (int?)null;

            // J1
            RepJ1.K2 = await CargarK2JugadorAsync(RepJ1.Jugador.Id, pid);
            // >>> Poblar partidos para SetsJugados
            if (pid.HasValue)
                RepJ1.K2.Partido = PartidosSeleccionados.First(p => p.Id == pid.Value);
            else
                RepJ1.K2.Partidos = PartidosSeleccionados.ToList();

            // ↓ sincronizar con subreporte de ataque:
            RepJ1.Ataque.K2 = RepJ1.K2;
            RepJ1.Ataque.TotalK2 = RepJ1.K2.PuntosJugados.Total;
            RepJ1.Ataque.EfectividadK2 = RepJ1.K2.PuntosJugados.Efect;

            // J2
            RepJ2.K2 = await CargarK2JugadorAsync(RepJ2.Jugador.Id, pid);
            if (pid.HasValue)
                RepJ2.K2.Partido = PartidosSeleccionados.First(p => p.Id == pid.Value);
            else
                RepJ2.K2.Partidos = PartidosSeleccionados.ToList();

            RepJ2.Ataque.K2 = RepJ2.K2;
            RepJ2.Ataque.TotalK2 = RepJ2.K2.PuntosJugados.Total;
            RepJ2.Ataque.EfectividadK2 = RepJ2.K2.PuntosJugados.Efect;

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
            var countsAll = await ContarResultadosRec(qJugador);
            var totalJugador = countsAll.Total;

            var porTipo_All = await ContarPorTipoRecepcion(qJugador);
            var sectores_All = await ContarPorSector(qJugador, totalJugador);

            var qF = qJugador.Where(e => e.TipoSaque == TipoSaque.Flotado);
            var flotCounts = await ContarResultadosRec(qF);
            var porTipo_F = await ContarPorTipoRecepcion(qF);
            var sectores_F = await ContarPorSector(qF, flotCounts.Total);

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
            var countsAll = await ContarResultadosAtk(qJugador);
            var totalJugador = countsAll.Total;

            var porAccion = await ContarPorAccion(qJugador);

            int getTotal(TipoAcciones a) =>
                porAccion.TryGetValue(a, out var c) ? c.Total : 0;

            int a1_2da = getTotal(TipoAcciones.Atq2daA1);
            int a6_2da = getTotal(TipoAcciones.Atq2daA6);
            int a5_2da = getTotal(TipoAcciones.Atq2daA5);
            int total2da = a1_2da + a6_2da + a5_2da;

            var familias = AgruparFamilias(porAccion);

            var totalK1Sin2daJugador = porAccion
                .Where(kv => kv.Key != TipoAcciones.PorAtras
                    && EsK1(kv.Key)
                    && !EsAtq2da(kv.Key))
                .Sum(kv => kv.Value.Total);

            var lados = await ContarPorLado(qJugador, totalJugador, totalK1Sin2daJugador);

            AtkCounts FamOrEmpty(string key) =>
                familias.TryGetValue(key, out var v) ? v : new AtkCounts();

            var atq2da = FamOrEmpty("Atq2da");
            var varios = FamOrEmpty("Varios");
            int variosCount = varios.Total;
            decimal variosEfect = varios.Efect;
            var k2Counts = new AtkCounts(
                atq2da.DP + varios.DP,
                atq2da.P + varios.P,
                atq2da.N + varios.N,
                atq2da.E + varios.E
            );
            var k2Total = k2Counts.Total;

            var k1Total = Math.Max(0, totalJugador - k2Total);

            int k1DP = countsAll.DP - k2Counts.DP;
            int k1P = countsAll.P - k2Counts.P;

            decimal efectK1Principal = k1Total > 0 ? (k1DP + k1P) / (decimal)k1Total : 0m;
            decimal efectK1De2da = atq2da.Total > 0 ? (atq2da.DP + atq2da.P) / (decimal)atq2da.Total : 0m;

            int k1Sin2daTotal = Math.Max(0, k1Total - atq2da.Total);
            int sin2daDP = k1DP - atq2da.DP;
            int sin2daP = k1P - atq2da.P;
            decimal efectK1Sin2da = k1Sin2daTotal > 0 ? (sin2daDP + sin2daP) / (decimal)k1Sin2daTotal : 0m;

            decimal efectAtk = countsAll.Efect;
            decimal pctK1 = totalJugador > 0 ? k1Total / (decimal)totalJugador : 0m;
            decimal pctNoK1 = 1m - pctK1;

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

                TotalK1 = k1Total,
                TotalK1Sin2da = k1Sin2daTotal,
                TotalK1De2da = atq2da.Total,
                PctK1 = pctK1,
                PctNoK1 = pctNoK1,
                EfectividadK1 = efectK1Principal,
                EfectividadAtaque = efectAtk,

                EfectividadK1Principal = efectK1Principal,
                EfectividadK1Sin2da = efectK1Sin2da,
                EfectividadK1De2da = efectK1De2da,

                TotalK2 = k2Total,
                EfectividadK2 = k2Counts.Efect,

                VariosCantidad = variosCount,
                VariosEfectividad = variosEfect,

                Total2da = total2da,
                A1_2da = a1_2da,
                A6_2da = a6_2da,
                A5_2da = a5_2da
            };
        }

        private static bool EsK1(TipoAcciones a)
        {
            var s = a.ToString();
            return s.StartsWith("Atq", StringComparison.OrdinalIgnoreCase)
                || s.StartsWith("Tl", StringComparison.OrdinalIgnoreCase)
                || s.StartsWith("Td", StringComparison.OrdinalIgnoreCase)
                || s.Equals("Varios", StringComparison.OrdinalIgnoreCase);
        }

        private static bool EsAtq2da(TipoAcciones a)
            => a.ToString().StartsWith("Atq2da", StringComparison.OrdinalIgnoreCase);

        private static async Task<AtkCounts> ContarResultadosAtk(IQueryable<EstadisticaAtaque> q)
        {
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
            int totalK1Sin2daJugador)
        {
            var raw = await q.GroupBy(e => new { e.Lado, e.Accion, e.Resultado })
                             .Select(g => new { g.Key.Lado, g.Key.Accion, g.Key.Resultado, S = g.Sum(x => x.Cantidad) })
                             .ToListAsync();

            var lados = new[] { TipoLado.Bueno, TipoLado.Medio, TipoLado.Atras };
            var lista = new List<AtaqueLado>();

            foreach (var lado in lados)
            {
                var porAccion = Enum.GetValues<TipoAcciones>().ToDictionary(a => a, a => new AtkCounts());
                var varillaPorAccion = Enum.GetValues<TipoAcciones>().ToDictionary(a => a, a => 0);
                var entreLineaPorAccion = Enum.GetValues<TipoAcciones>().ToDictionary(a => a, a => 0);

                var tot = new AtkCounts();
                int varT = 0, linT = 0;

                int sum2daEsteLado = 0;
                int k1Sin2daEsteLado = 0;

                foreach (var r in raw.Where(x => x.Lado == lado))
                {
                    if (r.Accion == TipoAcciones.PorAtras) continue;

                    var c = porAccion[r.Accion];
                    bool cuentaParaVE = r.Accion != TipoAcciones.Varios;

                    switch (r.Resultado)
                    {
                        case ResultadoAtaque.DoblePositivoV:
                            c.DP += r.S; tot.DP += r.S;
                            if (cuentaParaVE) { varT += r.S; varillaPorAccion[r.Accion] += r.S; }
                            break;
                        case ResultadoAtaque.PositivoV:
                            c.P += r.S; tot.P += r.S;
                            if (cuentaParaVE) { varT += r.S; varillaPorAccion[r.Accion] += r.S; }
                            break;
                        case ResultadoAtaque.NegativoV:
                            c.N += r.S; tot.N += r.S;
                            if (cuentaParaVE) { varT += r.S; varillaPorAccion[r.Accion] += r.S; }
                            break;
                        case ResultadoAtaque.DobleNegativoV:
                            c.E += r.S; tot.E += r.S;
                            if (cuentaParaVE) { varT += r.S; varillaPorAccion[r.Accion] += r.S; }
                            break;

                        case ResultadoAtaque.DoblePositivoE:
                            c.DP += r.S; tot.DP += r.S;
                            if (cuentaParaVE) { linT += r.S; entreLineaPorAccion[r.Accion] += r.S; }
                            break;
                        case ResultadoAtaque.PositivoE:
                            c.P += r.S; tot.P += r.S;
                            if (cuentaParaVE) { linT += r.S; entreLineaPorAccion[r.Accion] += r.S; }
                            break;
                        case ResultadoAtaque.NegativoE:
                            c.N += r.S; tot.N += r.S;
                            if (cuentaParaVE) { linT += r.S; entreLineaPorAccion[r.Accion] += r.S; }
                            break;
                        case ResultadoAtaque.DobleNegativoE:
                            c.E += r.S; tot.E += r.S;
                            if (cuentaParaVE) { linT += r.S; entreLineaPorAccion[r.Accion] += r.S; }
                            break;
                    }

                    if (r.Accion == TipoAcciones.Atq2daA1 ||
                        r.Accion == TipoAcciones.Atq2daA6 ||
                        r.Accion == TipoAcciones.Atq2daA5)
                    {
                        sum2daEsteLado += r.S;
                    }

                    if (EsK1(r.Accion) && !EsAtq2da(r.Accion))
                        k1Sin2daEsteLado += r.S;
                }

                if (tot.Total == 0) continue;

                var dictConDatos = porAccion.Where(kv => kv.Value.Total > 0)
                                            .ToDictionary(kv => kv.Key, kv => kv.Value);

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

            // Aceptar PartidoCompleto y Cierre
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

            if (rep.PuntosJugados.Total == 0 && totalK2Fallback > 0)
            {
                rep.PuntosJugados.DP = 0;
                rep.PuntosJugados.P = totalK2Fallback;
            }

            return rep;
        }

        private List<MapaAccionVM> BuildMapaAccionesFromLados(List<AtaqueLado> lados)
        {
            var acciones = new List<MapaAccionVM>();

            foreach (var lado in lados ?? Enumerable.Empty<AtaqueLado>())
            {
                foreach (var aa in lado.Acciones ?? Enumerable.Empty<AtaqueAccion>())
                {
                    var mapa = new MapaAccionVM
                    {
                        Nombre = aa.Accion.ToString(),
                        C = aa.C ?? new AtkCounts(),
                        Col = MapColFromAccion(aa.Accion),
                        Row = MapRowFromLado(lado.Lado),
                        TotalVarilla = aa.TotalVarilla,
                        TotalEntreLinea = aa.TotalEntreLinea
                    };

                    acciones.Add(mapa);
                }
            }

            return acciones;
        }

        private int MapRowFromLado(TipoLado lado) => lado switch
        {
            TipoLado.Bueno => 0,
            TipoLado.Medio => 1,
            TipoLado.Atras => 2,
            _ => 1
        };

        private int MapColFromAccion(TipoAcciones accion)
        {
            var s = accion.ToString().ToLowerInvariant();

            if (s.StartsWith("atq2da") || s.Contains("2da")) return 2;
            if (s.StartsWith("atq")) return 2;
            if (s.StartsWith("tl")) return 1;
            if (s.StartsWith("td")) return 1;
            if (s.StartsWith("poratras")) return 0;
            return 0;
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
        public int TotalEquipo { get; set; }
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
        public int TotalVarilla { get; set; }
        public int TotalEntreLinea { get; set; }
    }

    public class AtaqueLado
    {
        public TipoLado Lado { get; set; }
        public AtkCounts Totales { get; set; } = new();
        public decimal PctSobreJugador { get; set; }

        public int Total { get; set; }
        public int TotalVarilla { get; set; }
        public int TotalEntreLinea { get; set; }
        public decimal PctK1Sin2daSobreJugador { get; set; }

        public Dictionary<TipoAcciones, AtkCounts> PorAccion { get; set; } = new();
        public List<AtaqueAccion> Acciones { get; set; } = new();
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

    public class AtaquePlayerReport
    {
        public Jugador Jugador { get; set; } = default!;

        public AtkCounts Totales { get; set; } = new(0, 0, 0, 0);
        public decimal PctSobreEquipo { get; set; }
        public Dictionary<TipoAcciones, AtkCounts> PorAccion { get; set; } = new();
        public Dictionary<string, AtkCounts> Familias { get; set; } = new();
        public List<AtaqueLado> Lados { get; set; } = new();
        public int VariosCantidad { get; set; }
        public decimal VariosEfectividad { get; set; }
        public int Rol { get; set; } = 2;

        public int TotalK1 { get; set; }
        public int TotalK1Sin2da { get; set; }
        public int TotalK1De2da { get; set; }

        public decimal PctK1 { get; set; }
        public decimal PctNoK1 { get; set; }
        public K2Report? K2 { get; set; }

        public int TotalK2 { get; set; }
        public decimal EfectividadK2 { get; set; }

        public decimal EfectividadK1Principal { get; set; }
        public decimal EfectividadK1Sin2da { get; set; }
        public decimal EfectividadK1De2da { get; set; }

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

        // ====== Distribución específica de 2da (A1/A6/A5) ======
        public int Total2da { get; set; }
        public int A1_2da { get; set; }
        public int A6_2da { get; set; }
        public int A5_2da { get; set; }

        public string Dist2daInline =>
            Total2da > 0
                ? $"(A1 {Pct(A1_2da)} - A6 {Pct(A6_2da)} - A5 {Pct(A5_2da)})"
                : string.Empty;

        private string Pct(int part) =>
            Total2da > 0
                ? Math.Round(100.0 * part / (double)Total2da).ToString("0") + "%"
                : "0%";
    }

    // ==================== ViewModel K2 (único cuadro) ====================

    public class K2Counts
    {
        public int DP { get; set; }   // #
        public int P { get; set; }    // +
        public int N { get; set; }    // -
        public int DN { get; set; }   // =

        public int Total => DP + P + N + DN;

        public decimal Efect => Total > 0 ? (DP + P) / (decimal)Total : 0m;
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

        public Partido? Partido { get; set; }
        public List<Partido>? Partidos { get; set; }

        // ✅ Calcula bien para uno o varios partidos (2–0 ➜ 2; 2–1 ➜ 3; suma en multi)
        public int SetsJugados =>
            (Partidos != null && Partidos.Count > 0)
                ? Partidos.Sum(p => p.SetsGanadosDupla1 + p.SetsGanadosDupla2)
                : (Partido != null ? (Partido.SetsGanadosDupla1 + Partido.SetsGanadosDupla2) : 0);

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

        public AtaquePlayerReport Ataque { get; set; } = new();
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

    // Mapa Ataque VM
    public class MapaAtaqueViewModel
    {
        public string Titulo { get; set; } = "";
        public bool PlayerRight { get; set; }
        public int TotalAtaques { get; set; }
        public List<MapaAccionVM> Acciones { get; set; } = new();

        public List<(string Label, decimal Pct)>? Resumen { get; set; }
        public int ResumenCol { get; set; } = 2;
        public int ResumenRow { get; set; } = 1;
        public int TotalFamilia { get; set; }
        public int Rol { get; set; }
        public SandStats.Models.TipoLado Lado { get; set; }
        public int TotalVarilla { get; set; }
        public int TotalEntreLinea { get; set; }
    }

    public class MapaAccionVM
    {
        public string Nombre { get; set; } = "";
        public AtkCounts C { get; set; } = new();
        public int Col { get; set; }
        public int Row { get; set; }
        public int TotalVarilla { get; set; }
        public int TotalEntreLinea { get; set; }
        public MapaAccionVM() { }
        public MapaAccionVM(string nombre, AtkCounts counts, int col, int row)
        { Nombre = nombre; C = counts; Col = col; Row = row; }
    }
}
