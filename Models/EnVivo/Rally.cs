using System.ComponentModel.DataAnnotations;
using SandStats.Models;

namespace SandStats.Models.EnVivo
{
    public class Rally
    {
        public int Id { get; set; }

        [Required]
        public int SetEnVivoId { get; set; }
        public SetEnVivo? SetEnVivo { get; set; }

        [Required]
        [Display(Name = "Número de Rally")]
        public int NumeroRally { get; set; }

        [Display(Name = "Dupla Ganadora")]
        public int? DuplaGanadoraId { get; set; }
        public Dupla? DuplaGanadora { get; set; }

        public int MarcadorDupla1 { get; set; }
        public int MarcadorDupla2 { get; set; }

        public TipoCierreRally? TipoCierre { get; set; }

        public List<Accion> Acciones { get; set; } = new();
    }
}
