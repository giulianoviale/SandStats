using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScopeToEstadisticaK2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EstadisticaK2_PartidoId",
                table: "EstadisticaK2");

            migrationBuilder.AddColumn<int>(
                name: "DesdePunto",
                table: "EstadisticaK2",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Scope",
                table: "EstadisticaK2",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SetNumero",
                table: "EstadisticaK2",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaK2_PartidoId_JugadorId_Scope_SetNumero",
                table: "EstadisticaK2",
                columns: new[] { "PartidoId", "JugadorId", "Scope", "SetNumero" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EstadisticaK2_PartidoId_JugadorId_Scope_SetNumero",
                table: "EstadisticaK2");

            migrationBuilder.DropColumn(
                name: "DesdePunto",
                table: "EstadisticaK2");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "EstadisticaK2");

            migrationBuilder.DropColumn(
                name: "SetNumero",
                table: "EstadisticaK2");

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaK2_PartidoId",
                table: "EstadisticaK2",
                column: "PartidoId");
        }
    }
}
