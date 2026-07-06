namespace SandStats.Models.EnVivo
{
    public class ModificadorCombinada
    {
        public int Id { get; set; }

        public Fundamento FundamentoCargado { get; set; }

        public Calidad CalidadCargada { get; set; }

        public Fundamento FundamentoDerivado { get; set; }

        public Calidad? CalidadDerivada { get; set; }
    }
}
