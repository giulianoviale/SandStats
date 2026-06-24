using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class AgregaModuloEnVivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartidosEnVivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Torneo = table.Column<string>(type: "TEXT", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Dupla1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Dupla2Id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidosEnVivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartidosEnVivo_Duplas_Dupla1Id",
                        column: x => x.Dupla1Id,
                        principalTable: "Duplas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartidosEnVivo_Duplas_Dupla2Id",
                        column: x => x.Dupla2Id,
                        principalTable: "Duplas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SetsEnVivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartidoEnVivoId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeroSet = table.Column<int>(type: "INTEGER", nullable: false),
                    SacadorInicialJugadorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetsEnVivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetsEnVivo_Jugadores_SacadorInicialJugadorId",
                        column: x => x.SacadorInicialJugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SetsEnVivo_PartidosEnVivo_PartidoEnVivoId",
                        column: x => x.PartidoEnVivoId,
                        principalTable: "PartidosEnVivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rallies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SetEnVivoId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeroRally = table.Column<int>(type: "INTEGER", nullable: false),
                    DuplaGanadoraId = table.Column<int>(type: "INTEGER", nullable: true),
                    MarcadorDupla1 = table.Column<int>(type: "INTEGER", nullable: false),
                    MarcadorDupla2 = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rallies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rallies_Duplas_DuplaGanadoraId",
                        column: x => x.DuplaGanadoraId,
                        principalTable: "Duplas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rallies_SetsEnVivo_SetEnVivoId",
                        column: x => x.SetEnVivoId,
                        principalTable: "SetsEnVivo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Acciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RallyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Secuencia = table.Column<int>(type: "INTEGER", nullable: false),
                    JugadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Fundamento = table.Column<int>(type: "INTEGER", nullable: false),
                    Calidad = table.Column<int>(type: "INTEGER", nullable: false),
                    EsRejuego = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Acciones_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Acciones_Rallies_RallyId",
                        column: x => x.RallyId,
                        principalTable: "Rallies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesAtaque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Lado = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoAccion = table.Column<int>(type: "INTEGER", nullable: false),
                    ZonaDestino = table.Column<int>(type: "INTEGER", nullable: false),
                    EsVarilla = table.Column<bool>(type: "INTEGER", nullable: false),
                    EsEspecial = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesAtaque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesAtaque_Acciones_AccionId",
                        column: x => x.AccionId,
                        principalTable: "Acciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesRecepcion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccionId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoRecepcion = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesRecepcion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesRecepcion_Acciones_AccionId",
                        column: x => x.AccionId,
                        principalTable: "Acciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesSaque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ZonaSaque = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoSaque = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesSaque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesSaque_Acciones_AccionId",
                        column: x => x.AccionId,
                        principalTable: "Acciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Acciones_JugadorId",
                table: "Acciones",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Acciones_RallyId",
                table: "Acciones",
                column: "RallyId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesAtaque_AccionId",
                table: "DetallesAtaque",
                column: "AccionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesRecepcion_AccionId",
                table: "DetallesRecepcion",
                column: "AccionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesSaque_AccionId",
                table: "DetallesSaque",
                column: "AccionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartidosEnVivo_Dupla1Id",
                table: "PartidosEnVivo",
                column: "Dupla1Id");

            migrationBuilder.CreateIndex(
                name: "IX_PartidosEnVivo_Dupla2Id",
                table: "PartidosEnVivo",
                column: "Dupla2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Rallies_DuplaGanadoraId",
                table: "Rallies",
                column: "DuplaGanadoraId");

            migrationBuilder.CreateIndex(
                name: "IX_Rallies_SetEnVivoId",
                table: "Rallies",
                column: "SetEnVivoId");

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_PartidoEnVivoId",
                table: "SetsEnVivo",
                column: "PartidoEnVivoId");

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_SacadorInicialJugadorId",
                table: "SetsEnVivo",
                column: "SacadorInicialJugadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesAtaque");

            migrationBuilder.DropTable(
                name: "DetallesRecepcion");

            migrationBuilder.DropTable(
                name: "DetallesSaque");

            migrationBuilder.DropTable(
                name: "Acciones");

            migrationBuilder.DropTable(
                name: "Rallies");

            migrationBuilder.DropTable(
                name: "SetsEnVivo");

            migrationBuilder.DropTable(
                name: "PartidosEnVivo");
        }
    }
}
