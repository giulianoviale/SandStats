using System.ComponentModel.DataAnnotations;

namespace SandStats.Models
{
    public class EstadisticaAtaque
    {
        public int Id { get; set; }

        public int PartidoId { get; set; }
        public Partido? Partido { get; set; }

        public int JugadorId { get; set; }
        public Jugador? Jugador { get; set; }

        public TipoLado Lado { get; set; }
        public TipoAcciones Accion { get; set; } = new();
        public ResultadoAtaque Resultado { get; set; }
        public int Cantidad { get; set; }

        public DateTime FechaCarga { get; set; } = DateTime.Now;
        // NUEVOS (para soportar “últimos puntos”):
        public ScopeEstadistica Scope { get; set; } = ScopeEstadistica.PartidoCompleto;
        public int? DesdePunto { get; set; } // ej. 16 (sets a 21) u 11 (tie-break a 15)
        public int? SetNumero { get; set; }  // 1, 2 o 3 (tie-break)

    }
    public enum ScopeEstadistica
    {
        PartidoCompleto = 0, // comportamiento actual
        Cierre = 1           // “últimos puntos”
    }
    public enum TipoAcciones
    {
        Atq1,Atq2, Atq3, Atq4, Atq5, Atq6, Atq7, Atq8, Atq9,
        Tl1,Tl2,Tl3,Tl4,Tl5,Tl6,Tl7,Tl8,Tl9,
        Td1,Td2,Td3,Td4,Td5,Td6,Td7,Td8,Td9,
        Atq2da,Atq2daA1,Atq2daA6,Atq2daA5,Varios,PorAtras
    }
    public enum ResultadoAtaque
    {
        DoblePositivoV,
        DoblePositivoE,
        PositivoV,
        PositivoE,
        NegativoV,
        NegativoE,
        DobleNegativoV,
        DobleNegativoE
    }

    public enum TipoLado
    {
        Bueno,Medio,Atras
    }

}
