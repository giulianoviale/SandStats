using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class AjustesLogicaRally : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetsEnVivo_Jugadores_SacadorInicialJugadorId",
                table: "SetsEnVivo");

            migrationBuilder.RenameColumn(
                name: "SacadorInicialJugadorId",
                table: "SetsEnVivo",
                newName: "SacadorInicialDupla2JugadorId");

            migrationBuilder.RenameIndex(
                name: "IX_SetsEnVivo_SacadorInicialJugadorId",
                table: "SetsEnVivo",
                newName: "IX_SetsEnVivo_SacadorInicialDupla2JugadorId");

            migrationBuilder.AddColumn<int>(
                name: "SacadorInicialDupla1JugadorId",
                table: "SetsEnVivo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Complejo",
                table: "Acciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EsDe2da",
                table: "Acciones",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ModificadoresCombinadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FundamentoCargado = table.Column<int>(type: "INTEGER", nullable: false),
                    CalidadCargada = table.Column<int>(type: "INTEGER", nullable: false),
                    FundamentoDerivado = table.Column<int>(type: "INTEGER", nullable: false),
                    CalidadDerivada = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModificadoresCombinadas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_SacadorInicialDupla1JugadorId",
                table: "SetsEnVivo",
                column: "SacadorInicialDupla1JugadorId");

            migrationBuilder.CreateIndex(
                name: "UX_ModificadorCombinada",
                table: "ModificadoresCombinadas",
                columns: new[] { "FundamentoCargado", "CalidadCargada", "FundamentoDerivado" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SetsEnVivo_Jugadores_SacadorInicialDupla1JugadorId",
                table: "SetsEnVivo",
                column: "SacadorInicialDupla1JugadorId",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SetsEnVivo_Jugadores_SacadorInicialDupla2JugadorId",
                table: "SetsEnVivo",
                column: "SacadorInicialDupla2JugadorId",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetsEnVivo_Jugadores_SacadorInicialDupla1JugadorId",
                table: "SetsEnVivo");

            migrationBuilder.DropForeignKey(
                name: "FK_SetsEnVivo_Jugadores_SacadorInicialDupla2JugadorId",
                table: "SetsEnVivo");

            migrationBuilder.DropTable(
                name: "ModificadoresCombinadas");

            migrationBuilder.DropIndex(
                name: "IX_SetsEnVivo_SacadorInicialDupla1JugadorId",
                table: "SetsEnVivo");

            migrationBuilder.DropColumn(
                name: "SacadorInicialDupla1JugadorId",
                table: "SetsEnVivo");

            migrationBuilder.DropColumn(
                name: "Complejo",
                table: "Acciones");

            migrationBuilder.DropColumn(
                name: "EsDe2da",
                table: "Acciones");

            migrationBuilder.RenameColumn(
                name: "SacadorInicialDupla2JugadorId",
                table: "SetsEnVivo",
                newName: "SacadorInicialJugadorId");

            migrationBuilder.RenameIndex(
                name: "IX_SetsEnVivo_SacadorInicialDupla2JugadorId",
                table: "SetsEnVivo",
                newName: "IX_SetsEnVivo_SacadorInicialJugadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SetsEnVivo_Jugadores_SacadorInicialJugadorId",
                table: "SetsEnVivo",
                column: "SacadorInicialJugadorId",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
