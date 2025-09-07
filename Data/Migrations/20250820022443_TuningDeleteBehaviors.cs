using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Data.Migrations
{
    /// <inheritdoc />
    public partial class TuningDeleteBehaviors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EstadisticaAtaque_Jugadores_JugadorId",
                table: "EstadisticaAtaque");

            migrationBuilder.DropForeignKey(
                name: "FK_EstadisticaK2_Jugadores_JugadorId",
                table: "EstadisticaK2");

            migrationBuilder.DropForeignKey(
                name: "FK_EstadisticaRecepcion_Jugadores_JugadorId",
                table: "EstadisticaRecepcion");

            migrationBuilder.DropForeignKey(
                name: "FK_Jugadores_Duplas_DuplaId",
                table: "Jugadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Duplas_Dupla1Id",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Duplas_Dupla2Id",
                table: "Partidos");

            migrationBuilder.DropIndex(
                name: "IX_EstadisticaRecepcion_PartidoId_JugadorId_Scope_SetNumero",
                table: "EstadisticaRecepcion");

            migrationBuilder.DropIndex(
                name: "IX_EstadisticaK2_PartidoId_JugadorId_Scope_SetNumero",
                table: "EstadisticaK2");

            migrationBuilder.DropIndex(
                name: "IX_EstadisticaAtaque_PartidoId_JugadorId_Lado_Accion_Resultado_Scope_SetNumero",
                table: "EstadisticaAtaque");

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaRecepcion_PartidoId",
                table: "EstadisticaRecepcion",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaK2_PartidoId",
                table: "EstadisticaK2",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaAtaque_PartidoId",
                table: "EstadisticaAtaque",
                column: "PartidoId");

            migrationBuilder.AddForeignKey(
                name: "FK_EstadisticaAtaque_Jugadores_JugadorId",
                table: "EstadisticaAtaque",
                column: "JugadorId",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EstadisticaK2_Jugadores_JugadorId",
                table: "EstadisticaK2",
                column: "JugadorId",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EstadisticaRecepcion_Jugadores_JugadorId",
                table: "EstadisticaRecepcion",
                column: "JugadorId",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Jugadores_Duplas_DuplaId",
                table: "Jugadores",
                column: "DuplaId",
                principalTable: "Duplas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Duplas_Dupla1Id",
                table: "Partidos",
                column: "Dupla1Id",
                principalTable: "Duplas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Duplas_Dupla2Id",
                table: "Partidos",
                column: "Dupla2Id",
                principalTable: "Duplas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EstadisticaAtaque_Jugadores_JugadorId",
                table: "EstadisticaAtaque");

            migrationBuilder.DropForeignKey(
                name: "FK_EstadisticaK2_Jugadores_JugadorId",
                table: "EstadisticaK2");

            migrationBuilder.DropForeignKey(
                name: "FK_EstadisticaRecepcion_Jugadores_JugadorId",
                table: "EstadisticaRecepcion");

            migrationBuilder.DropForeignKey(
                name: "FK_Jugadores_Duplas_DuplaId",
                table: "Jugadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Duplas_Dupla1Id",
                table: "Partidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Partidos_Duplas_Dupla2Id",
                table: "Partidos");

            migrationBuilder.DropIndex(
                name: "IX_EstadisticaRecepcion_PartidoId",
                table: "EstadisticaRecepcion");

            migrationBuilder.DropIndex(
                name: "IX_EstadisticaK2_PartidoId",
                table: "EstadisticaK2");

            migrationBuilder.DropIndex(
                name: "IX_EstadisticaAtaque_PartidoId",
                table: "EstadisticaAtaque");

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaRecepcion_PartidoId_JugadorId_Scope_SetNumero",
                table: "EstadisticaRecepcion",
                columns: new[] { "PartidoId", "JugadorId", "Scope", "SetNumero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaK2_PartidoId_JugadorId_Scope_SetNumero",
                table: "EstadisticaK2",
                columns: new[] { "PartidoId", "JugadorId", "Scope", "SetNumero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EstadisticaAtaque_PartidoId_JugadorId_Lado_Accion_Resultado_Scope_SetNumero",
                table: "EstadisticaAtaque",
                columns: new[] { "PartidoId", "JugadorId", "Lado", "Accion", "Resultado", "Scope", "SetNumero" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EstadisticaAtaque_Jugadores_JugadorId",
                table: "EstadisticaAtaque",
                column: "JugadorId",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EstadisticaK2_Jugadores_JugadorId",
                table: "EstadisticaK2",
                column: "JugadorId",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EstadisticaRecepcion_Jugadores_JugadorId",
                table: "EstadisticaRecepcion",
                column: "JugadorId",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jugadores_Duplas_DuplaId",
                table: "Jugadores",
                column: "DuplaId",
                principalTable: "Duplas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Duplas_Dupla1Id",
                table: "Partidos",
                column: "Dupla1Id",
                principalTable: "Duplas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Partidos_Duplas_Dupla2Id",
                table: "Partidos",
                column: "Dupla2Id",
                principalTable: "Duplas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
