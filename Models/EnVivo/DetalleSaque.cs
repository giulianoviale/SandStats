using System.ComponentModel.DataAnnotations;
using SandStats.Models.SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class DetalleSaque
    {
        public int Id { get; set; }

        [Required]
        public int AccionId { get; set; }
        public Accion? Accion { get; set; }

        [Required]
        [Display(Name = "Zona de Saque")]
        public ZonaSaque ZonaSaque { get; set; }

        [Required]
        [Display(Name = "Tipo de Saque")]
        public TipoSaque TipoSaque { get; set; }
    }
}
