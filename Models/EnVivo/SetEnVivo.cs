using System.ComponentModel.DataAnnotations;
using SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class SetEnVivo
    {
        public int Id { get; set; }

        [Required]
        public int PartidoEnVivoId { get; set; }
        public PartidoEnVivo? PartidoEnVivo { get; set; }

        [Required]
        [Display(Name = "Número de Set")]
        public int NumeroSet { get; set; }

        [Required]
        [Display(Name = "Jugador que saca primero")]
        public int SacadorInicialJugadorId { get; set; }
        public Jugador? SacadorInicialJugador { get; set; }

        public List<Rally> Rallies { get; set; } = new();
    }
}
