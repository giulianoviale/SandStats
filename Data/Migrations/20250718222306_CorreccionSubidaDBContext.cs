using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorreccionSubidaDBContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EstadisticaAtaque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartidoId = table.Column<int>(type: "INTEGER", nullable: false),
                    JugadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Lado = table.Column<int>(type: "INTEGER", nullable: false),
                    Accion = table.Column<int>(type: "INTEGER", nullable: false),
                    Resultado = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadisticaAtaque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstadisticaAtaque_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstadisticaAtaque_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstadisticaK2",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartidoId = table.Column<int>(type: "INTEGER", nullable: false),
                    JugadorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadisticaK2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstadisticaK2_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstadisticaK2_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstadisticaRecepcion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartidoId = table.Column<int>(type: "INTEGER", nullable: false),
                    JugadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ZonaRecepcion = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoRecepcion = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoSaque = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadisticaRecepcion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstadisticaRecepcion_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstadisticaRecepcion_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaAtaque_JugadorId",
                table: "EstadisticaAtaque",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaAtaque_PartidoId",
                table: "EstadisticaAtaque",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaK2_JugadorId",
                table: "EstadisticaK2",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaK2_PartidoId",
                table: "EstadisticaK2",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaRecepcion_JugadorId",
                table: "EstadisticaRecepcion",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaRecepcion_PartidoId",
                table: "EstadisticaRecepcion",
                column: "PartidoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstadisticaAtaque");

            migrationBuilder.DropTable(
                name: "EstadisticaK2");

            migrationBuilder.DropTable(
                name: "EstadisticaRecepcion");
        }
    }
}
