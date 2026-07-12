using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class CierreDeSetYTipoCierre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuplaGanadoraId",
                table: "SetsEnVivo",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoCierre",
                table: "Rallies",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_DuplaGanadoraId",
                table: "SetsEnVivo",
                column: "DuplaGanadoraId");

            migrationBuilder.AddForeignKey(
                name: "FK_SetsEnVivo_Duplas_DuplaGanadoraId",
                table: "SetsEnVivo",
                column: "DuplaGanadoraId",
                principalTable: "Duplas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetsEnVivo_Duplas_DuplaGanadoraId",
                table: "SetsEnVivo");

            migrationBuilder.DropIndex(
                name: "IX_SetsEnVivo_DuplaGanadoraId",
                table: "SetsEnVivo");

            migrationBuilder.DropColumn(
                name: "DuplaGanadoraId",
                table: "SetsEnVivo");

            migrationBuilder.DropColumn(
                name: "TipoCierre",
                table: "Rallies");
        }
    }
}
