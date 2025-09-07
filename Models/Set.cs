using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SandStats.Models
{
    public class Set
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "ID Partido")]
        public int PartidoId { get; set; }

        [ForeignKey("PartidoId")]
        public Partido? Partido { get; set; }

        [Required]
        [Display(Name = "Numero de Set")]
        public int NumeroSet { get; set; } // 1, 2 o 3

        [Required]
        [Display(Name = "Puntos Dupla 1")]
        public int PuntosDupla1 { get; set; }

        [Required]
        [Display(Name = "Puntos Dupla 2")]
        public int PuntosDupla2 { get; set; }

        [Required]
        [Display(Name = "ID Dupla Ganadora")]
        public int GanadorDuplaId { get; set; }

        [ForeignKey("GanadorDuplaId")]
        public Dupla? Ganador { get; set; }
    }
}
