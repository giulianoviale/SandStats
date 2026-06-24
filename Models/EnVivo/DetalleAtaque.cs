using System.ComponentModel.DataAnnotations;
using SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class DetalleAtaque
    {
        public int Id { get; set; }

        [Required]
        public int AccionId { get; set; }
        public Accion? Accion { get; set; }

        [Required]
        [Display(Name = "Lado")]
        public TipoLado Lado { get; set; }

        [Required]
        [Display(Name = "Tipo de Acción")]
        public TipoAcciones TipoAccion { get; set; }

        [Required]
        [Display(Name = "Zona Destino")]
        public ZonaCancha ZonaDestino { get; set; }

        [Display(Name = "Es Varilla")]
        public bool EsVarilla { get; set; }

        [Display(Name = "Es Especial")]
        public bool EsEspecial { get; set; }
    }
}
