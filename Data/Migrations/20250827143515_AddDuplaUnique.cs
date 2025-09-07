using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDuplaUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Duplas_Jugador1Id",
                table: "Duplas");

            migrationBuilder.CreateIndex(
                name: "UX_Dupla_J1_J2",
                table: "Duplas",
                columns: new[] { "Jugador1Id", "Jugador2Id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Dupla_J1_J2",
                table: "Duplas");

            migrationBuilder.CreateIndex(
                name: "IX_Duplas_Jugador1Id",
                table: "Duplas",
                column: "Jugador1Id");
        }
    }
}
