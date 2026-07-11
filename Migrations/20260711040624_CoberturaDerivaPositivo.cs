using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class CoberturaDerivaPositivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 9,
                column: "CalidadDerivada",
                value: 4);

            migrationBuilder.UpdateData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 14,
                column: "CalidadDerivada",
                value: 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 9,
                column: "CalidadDerivada",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 14,
                column: "CalidadDerivada",
                value: 3);
        }
    }
}
