using System.ComponentModel.DataAnnotations;
using SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class Accion
    {
        public int Id { get; set; }

        [Required]
        public int RallyId { get; set; }
        public Rally? Rally { get; set; }

        [Required]
        [Display(Name = "Secuencia")]
        public int Secuencia { get; set; }

        [Required]
        [Display(Name = "Jugador")]
        public int JugadorId { get; set; }
        public Jugador? Jugador { get; set; }

        [Required]
        public Fundamento Fundamento { get; set; }

        [Required]
        public Calidad Calidad { get; set; }

        [Required]
        public Complejo Complejo { get; set; }

        public bool EsDe2da { get; set; }

        public bool EsRejuego { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        public DetalleSaque? DetalleSaque { get; set; }
        public DetalleRecepcion? DetalleRecepcion { get; set; }
        public DetalleAtaque? DetalleAtaque { get; set; }
    }
}
