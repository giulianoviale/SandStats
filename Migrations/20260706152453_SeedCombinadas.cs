using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class SeedCombinadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ModificadoresCombinadas",
                columns: new[] { "Id", "CalidadCargada", "CalidadDerivada", "FundamentoCargado", "FundamentoDerivado" },
                values: new object[,]
                {
                    { 1, 5, 1, 1, 0 },
                    { 2, 4, 1, 1, 0 },
                    { 3, 3, 3, 1, 0 },
                    { 4, 2, 2, 1, 0 },
                    { 5, 1, 4, 1, 0 },
                    { 6, 0, 5, 1, 0 },
                    { 7, 5, 2, 3, 2 },
                    { 8, 4, 1, 3, 2 },
                    { 9, 2, 3, 3, 2 },
                    { 10, 1, 4, 3, 2 },
                    { 11, 0, 5, 3, 2 },
                    { 12, 5, 1, 4, 2 },
                    { 13, 4, 1, 4, 2 },
                    { 14, 3, 3, 4, 2 },
                    { 15, 2, 4, 4, 2 },
                    { 16, 1, 4, 4, 2 },
                    { 17, 0, 5, 4, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ModificadoresCombinadas",
                keyColumn: "Id",
                keyValue: 17);
        }
    }
}
