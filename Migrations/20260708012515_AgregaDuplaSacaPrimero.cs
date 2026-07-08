using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class AgregaDuplaSacaPrimero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DuplaQueSacaPrimeroId",
                table: "SetsEnVivo",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_DuplaQueSacaPrimeroId",
                table: "SetsEnVivo",
                column: "DuplaQueSacaPrimeroId");

            migrationBuilder.AddForeignKey(
                name: "FK_SetsEnVivo_Duplas_DuplaQueSacaPrimeroId",
                table: "SetsEnVivo",
                column: "DuplaQueSacaPrimeroId",
                principalTable: "Duplas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SetsEnVivo_Duplas_DuplaQueSacaPrimeroId",
                table: "SetsEnVivo");

            migrationBuilder.DropIndex(
                name: "IX_SetsEnVivo_DuplaQueSacaPrimeroId",
                table: "SetsEnVivo");

            migrationBuilder.DropColumn(
                name: "DuplaQueSacaPrimeroId",
                table: "SetsEnVivo");
        }
    }
}
