using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Duplas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Alias = table.Column<string>(type: "TEXT", nullable: false),
                    Jugador1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Jugador2Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duplas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jugadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Apellido = table.Column<string>(type: "TEXT", nullable: false),
                    Apodo = table.Column<string>(type: "TEXT", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Altura = table.Column<decimal>(type: "TEXT", nullable: false),
                    Peso = table.Column<decimal>(type: "TEXT", nullable: false),
                    ManoHabil = table.Column<string>(type: "TEXT", nullable: false),
                    RolPrincipal = table.Column<int>(type: "INTEGER", nullable: false),
                    Nacionalidad = table.Column<string>(type: "TEXT", nullable: false),
                    ImagenPerfilPath = table.Column<string>(type: "TEXT", nullable: false),
                    DuplaId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jugadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jugadores_Duplas_DuplaId",
                        column: x => x.DuplaId,
                        principalTable: "Duplas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Duplas_Jugador1Id",
                table: "Duplas",
                column: "Jugador1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Duplas_Jugador2Id",
                table: "Duplas",
                column: "Jugador2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Jugadores_DuplaId",
                table: "Jugadores",
                column: "DuplaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Duplas_Jugadores_Jugador1Id",
                table: "Duplas",
                column: "Jugador1Id",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Duplas_Jugadores_Jugador2Id",
                table: "Duplas",
                column: "Jugador2Id",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Duplas_Jugadores_Jugador1Id",
                table: "Duplas");

            migrationBuilder.DropForeignKey(
                name: "FK_Duplas_Jugadores_Jugador2Id",
                table: "Duplas");

            migrationBuilder.DropTable(
                name: "Jugadores");

            migrationBuilder.DropTable(
                name: "Duplas");
        }
    }
}
