using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SandStats.Models
{
    public class Partido
    {
        public int Id { get; set; }

        [Required]
        public string Torneo { get; set; }

        [Required]
        [Display(Name = "Dupla 1")]

        public int Dupla1Id { get; set; }

        [ForeignKey("Dupla1Id")]

        public Dupla? Dupla1 { get; set; }

        [Required]
        [Display(Name = "Dupla 2")]

        public int Dupla2Id { get; set; }

        [ForeignKey("Dupla2Id")]
        public Dupla? Dupla2 { get; set; }

        [Required]
        public DateTime Fecha { get; set; }


        [Required]
        public Clima Clima { get; set; }

        [Required]
        [Display(Name = "Resultado Dupla 1")]

        public int SetsGanadosDupla1 { get; set; } // 2 si gana, 1 si pierde, etc.

        [Required]
        [Display(Name = "Resultado Dupla 2")]

        public int SetsGanadosDupla2 { get; set; }
        [NotMapped]
        public string Descripcion =>
        $"{Torneo} - {Fecha.ToShortDateString()} - {Dupla1?.Alias} vs {Dupla2?.Alias}";

        // ✅ Propiedad calculada (no se guarda en DB)
        [NotMapped]
        public int SetsJugados => SetsGanadosDupla1 + SetsGanadosDupla2;
        public List<Set> Sets { get; set; } = new List<Set>();
        [NotMapped]
        public string Resultado => $"{SetsGanadosDupla1}-{SetsGanadosDupla2}";
        [MaxLength(1000)]
        public string? Observaciones { get; set; }
        [MaxLength(2048)]
        [Url]
        public string? VideoUrl { get; set; }

    }

    public enum Clima
    {
        Normal,
        NormalNocturno,
        Calor,
        Lluvia,
        Viento
    }
}
