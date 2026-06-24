using System.ComponentModel.DataAnnotations;
using SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class PartidoEnVivo
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Torneo")]
        public string Torneo { get; set; } = string.Empty;

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [Display(Name = "Dupla 1")]
        public int Dupla1Id { get; set; }
        public Dupla? Dupla1 { get; set; }

        [Required]
        [Display(Name = "Dupla 2")]
        public int Dupla2Id { get; set; }
        public Dupla? Dupla2 { get; set; }

        public List<SetEnVivo> Sets { get; set; } = new();
    }
}
