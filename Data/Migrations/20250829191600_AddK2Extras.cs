using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddK2Extras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Agregados",
                table: "EstadisticaK2",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BloqueadoAtqa1",
                table: "EstadisticaK2",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BloqueadoAtqa5",
                table: "EstadisticaK2",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BloqueadoAtqa6",
                table: "EstadisticaK2",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ErroresVarios",
                table: "EstadisticaK2",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetsJugados",
                table: "EstadisticaK2",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Agregados",
                table: "EstadisticaK2");

            migrationBuilder.DropColumn(
                name: "BloqueadoAtqa1",
                table: "EstadisticaK2");

            migrationBuilder.DropColumn(
                name: "BloqueadoAtqa5",
                table: "EstadisticaK2");

            migrationBuilder.DropColumn(
                name: "BloqueadoAtqa6",
                table: "EstadisticaK2");

            migrationBuilder.DropColumn(
                name: "ErroresVarios",
                table: "EstadisticaK2");

            migrationBuilder.DropColumn(
                name: "SetsJugados",
                table: "EstadisticaK2");
        }
    }
}
