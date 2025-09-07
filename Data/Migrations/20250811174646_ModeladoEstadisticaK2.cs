using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModeladoEstadisticaK2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cantidad",
                table: "EstadisticaK2",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCarga",
                table: "EstadisticaK2",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Fuente",
                table: "EstadisticaK2",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Resultado",
                table: "EstadisticaK2",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cantidad",
                table: "EstadisticaK2");

            migrationBuilder.DropColumn(
                name: "FechaCarga",
                table: "EstadisticaK2");

            migrationBuilder.DropColumn(
                name: "Fuente",
                table: "EstadisticaK2");

            migrationBuilder.DropColumn(
                name: "Resultado",
                table: "EstadisticaK2");
        }
    }
}
