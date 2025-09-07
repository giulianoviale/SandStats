namespace SandStats.Models
{
    public class VideoLinksJugadorPartido
    {
        public int Id { get; set; }

        // Claves foráneas
        public int PartidoId { get; set; }
        public Partido Partido { get; set; } = default!;

        public int JugadorId { get; set; }
        public Jugador Jugador { get; set; } = default!;

        // Los 3 links
        public string? LinkK1 { get; set; }
        public string? LinkK2 { get; set; }
        public string? LinkSaque { get; set; }
    }
}
