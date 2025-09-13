namespace SandStats.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace SandStats.Models
    {
        public class EstadisticaRecepcion
        {
            public int Id { get; set; }

            [Required]
            public int PartidoId { get; set; }

            [ForeignKey("PartidoId")]
            public Partido? Partido { get; set; }

            [Required]
            public int JugadorId { get; set; }

            [ForeignKey("JugadorId")]
            public Jugador? Jugador { get; set; }


            // usando System.ComponentModel.DataAnnotations.Schema;
            [Column(TypeName = "timestamp with time zone")]
            public DateTime FechaCarga { get; set; } = DateTime.UtcNow;


            public ZonaSaque ZonaRecepcion { get; set; }
            public TipoRecepcion TipoRecepcion { get; set; }
            public TipoSaque TipoSaque { get; set; }
            public ResultadoRecepcion ResultadoRecepcion { get; set; }
            // === NUEVO ===
            public ScopeEstadistica Scope { get; set; } = ScopeEstadistica.PartidoCompleto;
            public int? DesdePunto { get; set; }   // 16 o 11 (TB)
            public int? SetNumero { get; set; }    // 1, 2 o 3 (TB)

        }
        public enum TipoSaque
        {
            Flotado,
            Potencia
        }
        public enum TipoRecepcion
        {
            Adelante,Cuerpo,Externo,Interno
        }
        public enum ZonaSaque
        {
            Zona1 = 1,
            Zona6 = 6,
            Zona5 = 5
        }
        public enum ResultadoRecepcion
        {
            doblePositivo, positivo, negativo, dobleNegativo
        }
    }

}
