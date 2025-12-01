using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class AddJugadorLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JugadorLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JugadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    LinkK1 = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    LinkK2 = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    LinkSaque = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    LinkArmado = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    LinkBloqueo = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true),
                    LinkExtra = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JugadorLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JugadorLinks_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JugadorLinks_JugadorId",
                table: "JugadorLinks",
                column: "JugadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JugadorLinks");
        }
    }
}
