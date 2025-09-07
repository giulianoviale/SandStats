using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SandStats.Models
{
    public class EstadisticaK2
    {
        public int Id { get; set; }

        [Required]
        public int PartidoId { get; set; }
        public Partido? Partido { get; set; }

        [Required]
        public int JugadorId { get; set; }
        public Jugador? Jugador { get; set; }

        [Required]
        public FuenteK2 Fuente { get; set; }  // Flotado / Potencia / Bloqueo

        [Required]
        public ResultadoK2 Resultado { get; set; } // # + - =

        [Range(0, int.MaxValue)]
        public int Cantidad { get; set; }

        public DateTime FechaCarga { get; set; } = DateTime.Now;
        // === NUEVOS: para “últimos puntos” ===
        public ScopeEstadistica Scope { get; set; } = ScopeEstadistica.PartidoCompleto;
        public int? DesdePunto { get; set; }   // 16 o 11 (TB)
        public int? Agregados { get; set; }
        public int? ErroresVarios { get; set; }
        public int? SetsJugados { get; set; }
        public int? SetNumero { get; set; }    // 1, 2 o 3 (tie-break)
        public int? BloqueadoAtqa1 { get; set; }
        public int? BloqueadoAtqa6 { get; set; }
        public int? BloqueadoAtqa5 { get; set; }
    }

    public enum FuenteK2
    {
        PuntosJugados,
        SaqueFlotado,
        SaquePotencia,
        BloqueoAtqA1,
        BloqueoAtqA6,
        BloqueoAtqA5,
    }

    public enum ResultadoK2
    {
        DoblePositivo, // #
        Positivo,      // +
        Negativo,      // -
        DobleNegativo  // =
    }
}
