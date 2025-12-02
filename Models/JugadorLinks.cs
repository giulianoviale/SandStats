using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SandStats.Models
{
    public class JugadorLinks
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Jugador))]
        public int JugadorId { get; set; }

        public Jugador Jugador { get; set; } = default!;

        [MaxLength(2048)]
        public string? LinkK1 { get; set; }

        [MaxLength(2048)]
        public string? LinkK2 { get; set; }
        
        [MaxLength(2048)]
        public string? LinkSaque { get; set; }

        [MaxLength(2048)]
        public string? LinkArmado { get; set; }

        [MaxLength(2048)]
        public string? LinkBloqueo { get; set; }

        [MaxLength(2048)]
        public string? LinkExtra { get; set; }
    }
}
