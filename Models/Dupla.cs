using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SandStats.Models
{
    public class Dupla
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un jugador.")]
        [Display(Name = "Jugador 1")]
        public int Jugador1Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un jugador.")]
        [Display(Name = "Jugador 2")]
        public int Jugador2Id { get; set; }

        [Display(Name = "Alias")]
        public string? Alias { get; set; }

        // Relaciones
        public Jugador? Jugador1 { get; set; }
        public Jugador? Jugador2 { get; set; }
        public string? Nombre => $"{Jugador1?.NombreCompleto} y {Jugador2?.NombreCompleto}";
        [NotMapped]
        public List<Jugador> Jugadores => new() { Jugador1!, Jugador2! };
    }
}
