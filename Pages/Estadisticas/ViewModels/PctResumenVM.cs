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
    }
}
