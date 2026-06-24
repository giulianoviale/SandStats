using System.ComponentModel.DataAnnotations;
using SandStats.Models.SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class DetalleRecepcion
    {
        public int Id { get; set; }

        [Required]
        public int AccionId { get; set; }
        public Accion? Accion { get; set; }

        [Required]
        [Display(Name = "Tipo de Recepción")]
        public TipoRecepcion TipoRecepcion { get; set; }
    }
}
