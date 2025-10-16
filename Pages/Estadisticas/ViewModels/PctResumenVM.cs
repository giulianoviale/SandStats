using System;
using System.Collections.Generic;

namespace SandStats.Pages.Estadisticas
{
    public class PctResumenItem
    {
        public string Label { get; init; } = "";
        public double Pct { get; init; }   // 0..100
    }

    public class PctResumenVM
    {
        public List<PctResumenItem> Items { get; } = new();
        public int TotalAcciones { get; init; }

        // ===== Distribución específica para "Ataques de 2da" =====
        public int? Total2da { get; init; }  // total pelotas de 2da (A1 + A6 + A5)
        public int? A1 { get; init; }
        public int? A6 { get; init; }
        public int? A5 { get; init; }

        // Conveniencias ya formateadas
        public string PctA1 => FormatPct(A1, Total2da);
        public string PctA6 => FormatPct(A6, Total2da);
        public string PctA5 => FormatPct(A5, Total2da);

        // Texto listo para el encabezado
        public string Dist2daInline =>
            (Total2da ?? 0) > 0
                ? $"(A1 {PctA1} - A6 {PctA6} - A5 {PctA5})"
                : string.Empty;

        private static string FormatPct(int? part, int? total) =>
            (total ?? 0) > 0
                ? Math.Round(100.0 * (part ?? 0) / (double)total!.Value).ToString("0") + "%"
                : "0%";
    }
}
