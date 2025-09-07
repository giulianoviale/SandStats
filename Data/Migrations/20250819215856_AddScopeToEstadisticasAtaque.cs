using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScopeToEstadisticasAtaque : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EstadisticaAtaque_PartidoId",
                table: "EstadisticaAtaque");

            migrationBuilder.AddColumn<int>(
                name: "DesdePunto",
                table: "EstadisticaAtaque",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Scope",
                table: "EstadisticaAtaque",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SetNumero",
                table: "EstadisticaAtaque",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaAtaque_PartidoId_JugadorId_Lado_Accion_Resultado_Scope_SetNumero",
                table: "EstadisticaAtaque",
                columns: new[] { "PartidoId", "JugadorId", "Lado", "Accion", "Resultado", "Scope", "SetNumero" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EstadisticaAtaque_PartidoId_JugadorId_Lado_Accion_Resultado_Scope_SetNumero",
                table: "EstadisticaAtaque");

            migrationBuilder.DropColumn(
                name: "DesdePunto",
                table: "EstadisticaAtaque");

            migrationBuilder.DropColumn(
                name: "Scope",
                table: "EstadisticaAtaque");

            migrationBuilder.DropColumn(
                name: "SetNumero",
                table: "EstadisticaAtaque");

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaAtaque_PartidoId",
                table: "EstadisticaAtaque",
                column: "PartidoId");
        }
    }
}
