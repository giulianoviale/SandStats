using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScopeToEstadisticaRecepcion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EstadisticaRecepcion_PartidoId",
                table: "EstadisticaRecepcion");

            migrationBuilder.AddColumn<int>(
                name: "DesdePunto",
                table: "EstadisticaRecepcion",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Scope",
                table: "EstadisticaRecepcion",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SetNumero",
                table: "EstadisticaRecepcion",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaRecepcion_PartidoId_JugadorId_Scope_SetNumero",
                table: "EstadisticaRecepcion",
                columns: new[] { "PartidoId", "JugadorId", "Scope", "SetNumero" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EstadisticaRecepcion_PartidoId_JugadorId_Scope_SetNumero",
                table: "EstadisticaRecepcion");

            migrationBuilder.DropColumn(
                name: "DesdePunto",
                table: "EstadisticaRecepcion");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "EstadisticaRecepcion");

            migrationBuilder.DropColumn(
                name: "SetNumero",
                table: "EstadisticaRecepcion");

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaRecepcion_PartidoId",
                table: "EstadisticaRecepcion",
                column: "PartidoId");
        }
    }
}
