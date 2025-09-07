using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPosicionJugador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Posicion",
                table: "Jugadores",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Posicion",
                table: "Jugadores");
        }
    }
}
