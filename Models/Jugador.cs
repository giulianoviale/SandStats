using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SandStats.Models
{
    public class Jugador
    {
        public int Id { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido}";

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Apellido { get; set; } = string.Empty;

        public string? Apodo { get; set; }

        [Display(Name = "Rol Principal")]
        [Required]
        public RolJugador RolPrincipal { get; set; } // rol natural del jugador

        public string? Nacionalidad { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha De Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [Range(100, 250, ErrorMessage = "Altura debe estar entre 100 y 250 cm")]
        public int? Altura { get; set; } // en cm

        [Range(30, 150, ErrorMessage = "Peso debe estar entre 30 y 150 kg")]
        public int? Peso { get; set; } // en kg

        [Display(Name = "Mano Habil")]
        public string? ManoHabil { get; set; } // "Derecha", "Izquierda", "Ambas"

        public string? ImagenPerfilPath { get; set; }

        // Relación con Dupla
        public int? DuplaId { get; set; }
        public Dupla? Dupla { get; set; }
        public PosicionJugador Posicion { get; set; }
    }
    public enum PosicionJugador
    {
        Bloqueador = 0,
        Defensor = 1
    }
    public enum RolJugador
    {
        Rol4= 4,
        Rol2= 2
    }
}
