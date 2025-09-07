using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoLinksJugadorPartido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideoLinksJugadorPartido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartidoId = table.Column<int>(type: "INTEGER", nullable: false),
                    JugadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    LinkK1 = table.Column<string>(type: "TEXT", nullable: true),
                    LinkK2 = table.Column<string>(type: "TEXT", nullable: true),
                    LinkSaque = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLinksJugadorPartido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoLinksJugadorPartido_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoLinksJugadorPartido_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideoLinksJugadorPartido_JugadorId",
                table: "VideoLinksJugadorPartido",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoLinksJugadorPartido_PartidoId",
                table: "VideoLinksJugadorPartido",
                column: "PartidoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoLinksJugadorPartido");
        }
    }
}
