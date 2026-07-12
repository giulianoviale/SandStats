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
        [Display(Name = "Sacador inicial Dupla 1")]
        public int SacadorInicialDupla1JugadorId { get; set; }
        public Jugador? SacadorInicialDupla1Jugador { get; set; }

        [Required]
        [Display(Name = "Sacador inicial Dupla 2")]
        public int SacadorInicialDupla2JugadorId { get; set; }
        public Jugador? SacadorInicialDupla2Jugador { get; set; }

        [Required]
        [Display(Name = "Dupla que saca primero")]
        public int DuplaQueSacaPrimeroId { get; set; }
        public Dupla? DuplaQueSacaPrimero { get; set; }

        public int? DuplaGanadoraId { get; set; }
        public Dupla? DuplaGanadora { get; set; }

        public List<Rally> Rallies { get; set; } = new();
    }
}
