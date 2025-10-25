using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    MustResetPassword = table.Column<bool>(type: "INTEGER", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: true),
                    JugadorId = table.Column<int>(type: "INTEGER", nullable: true),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Duplas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Jugador1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Jugador2Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Alias = table.Column<string>(type: "TEXT", nullable: true)
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
                    Apodo = table.Column<string>(type: "TEXT", nullable: true),
                    RolPrincipal = table.Column<int>(type: "INTEGER", nullable: false),
                    Nacionalidad = table.Column<string>(type: "TEXT", nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Altura = table.Column<int>(type: "INTEGER", nullable: true),
                    Peso = table.Column<int>(type: "INTEGER", nullable: true),
                    ManoHabil = table.Column<string>(type: "TEXT", nullable: true),
                    ImagenPerfilPath = table.Column<string>(type: "TEXT", nullable: true),
                    DuplaId = table.Column<int>(type: "INTEGER", nullable: true),
                    Posicion = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jugadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jugadores_Duplas_DuplaId",
                        column: x => x.DuplaId,
                        principalTable: "Duplas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Partidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Torneo = table.Column<string>(type: "TEXT", nullable: false),
                    Dupla1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Dupla2Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Clima = table.Column<int>(type: "INTEGER", nullable: false),
                    SetsGanadosDupla1 = table.Column<int>(type: "INTEGER", nullable: false),
                    SetsGanadosDupla2 = table.Column<int>(type: "INTEGER", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    VideoUrl = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partidos_Duplas_Dupla1Id",
                        column: x => x.Dupla1Id,
                        principalTable: "Duplas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidos_Duplas_Dupla2Id",
                        column: x => x.Dupla2Id,
                        principalTable: "Duplas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Scope = table.Column<int>(type: "INTEGER", nullable: false),
                    DesdePunto = table.Column<int>(type: "INTEGER", nullable: true),
                    SetNumero = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadisticaAtaque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstadisticaAtaque_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    JugadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Fuente = table.Column<int>(type: "INTEGER", nullable: false),
                    Resultado = table.Column<int>(type: "INTEGER", nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Scope = table.Column<int>(type: "INTEGER", nullable: false),
                    DesdePunto = table.Column<int>(type: "INTEGER", nullable: true),
                    Agregados = table.Column<int>(type: "INTEGER", nullable: true),
                    ErroresVarios = table.Column<int>(type: "INTEGER", nullable: true),
                    SetsJugados = table.Column<int>(type: "INTEGER", nullable: true),
                    SetNumero = table.Column<int>(type: "INTEGER", nullable: true),
                    BloqueadoAtqa1 = table.Column<int>(type: "INTEGER", nullable: true),
                    BloqueadoAtqa6 = table.Column<int>(type: "INTEGER", nullable: true),
                    BloqueadoAtqa5 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadisticaK2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstadisticaK2_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    FechaCarga = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ZonaRecepcion = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoRecepcion = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoSaque = table.Column<int>(type: "INTEGER", nullable: false),
                    ResultadoRecepcion = table.Column<int>(type: "INTEGER", nullable: false),
                    Scope = table.Column<int>(type: "INTEGER", nullable: false),
                    DesdePunto = table.Column<int>(type: "INTEGER", nullable: true),
                    SetNumero = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadisticaRecepcion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstadisticaRecepcion_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstadisticaRecepcion_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartidoId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeroSet = table.Column<int>(type: "INTEGER", nullable: false),
                    PuntosDupla1 = table.Column<int>(type: "INTEGER", nullable: false),
                    PuntosDupla2 = table.Column<int>(type: "INTEGER", nullable: false),
                    GanadorDuplaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sets_Duplas_GanadorDuplaId",
                        column: x => x.GanadorDuplaId,
                        principalTable: "Duplas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sets_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoLinksJugadorPartido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartidoId = table.Column<int>(type: "INTEGER", nullable: false),
                    JugadorId = table.Column<int>(type: "INTEGER", nullable: false),
                    LinkK1 = table.Column<string>(type: "TEXT", nullable: true),
                    LinkK2 = table.Column<string>(type: "TEXT", nullable: true),
                    LinkSaque = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLinksJugadorPartido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoLinksJugadorPartido_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoLinksJugadorPartido_Partidos_PartidoId",
                        column: x => x.PartidoId,
                        principalTable: "Partidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Duplas_Jugador2Id",
                table: "Duplas",
                column: "Jugador2Id");

            migrationBuilder.CreateIndex(
                name: "UX_Dupla_J1_J2",
                table: "Duplas",
                columns: new[] { "Jugador1Id", "Jugador2Id" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Jugadores_DuplaId",
                table: "Jugadores",
                column: "DuplaId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_Dupla1Id",
                table: "Partidos",
                column: "Dupla1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_Dupla2Id",
                table: "Partidos",
                column: "Dupla2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_GanadorDuplaId",
                table: "Sets",
                column: "GanadorDuplaId");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_PartidoId",
                table: "Sets",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoLinksJugadorPartido_JugadorId",
                table: "VideoLinksJugadorPartido",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoLinksJugadorPartido_PartidoId",
                table: "VideoLinksJugadorPartido",
                column: "PartidoId");

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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "EstadisticaAtaque");

            migrationBuilder.DropTable(
                name: "EstadisticaK2");

            migrationBuilder.DropTable(
                name: "EstadisticaRecepcion");

            migrationBuilder.DropTable(
                name: "Sets");

            migrationBuilder.DropTable(
                name: "VideoLinksJugadorPartido");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Partidos");

            migrationBuilder.DropTable(
                name: "Jugadores");

            migrationBuilder.DropTable(
                name: "Duplas");
        }
    }
}
