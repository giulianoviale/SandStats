using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class InicialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    MustResetPassword = table.Column<bool>(type: "boolean", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    JugadorId = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModificadoresCombinadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FundamentoCargado = table.Column<int>(type: "integer", nullable: false),
                    CalidadCargada = table.Column<int>(type: "integer", nullable: false),
                    FundamentoDerivado = table.Column<int>(type: "integer", nullable: false),
                    CalidadDerivada = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModificadoresCombinadas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
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
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
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
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
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
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
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
                name: "Acciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RallyId = table.Column<int>(type: "integer", nullable: false),
                    Secuencia = table.Column<int>(type: "integer", nullable: false),
                    JugadorId = table.Column<int>(type: "integer", nullable: false),
                    Fundamento = table.Column<int>(type: "integer", nullable: false),
                    Calidad = table.Column<int>(type: "integer", nullable: true),
                    Complejo = table.Column<int>(type: "integer", nullable: false),
                    EsDe2da = table.Column<bool>(type: "boolean", nullable: false),
                    EsRejuego = table.Column<bool>(type: "boolean", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DetallesAtaque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccionId = table.Column<int>(type: "integer", nullable: false),
                    Lado = table.Column<int>(type: "integer", nullable: false),
                    TipoAccion = table.Column<int>(type: "integer", nullable: false),
                    ZonaDestino = table.Column<int>(type: "integer", nullable: false),
                    EsVarilla = table.Column<bool>(type: "boolean", nullable: false),
                    EsEspecial = table.Column<bool>(type: "boolean", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccionId = table.Column<int>(type: "integer", nullable: false),
                    TipoRecepcion = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccionId = table.Column<int>(type: "integer", nullable: false),
                    ZonaSaque = table.Column<int>(type: "integer", nullable: false),
                    TipoSaque = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Duplas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Jugador1Id = table.Column<int>(type: "integer", nullable: false),
                    Jugador2Id = table.Column<int>(type: "integer", nullable: false),
                    Alias = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duplas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jugadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Apellido = table.Column<string>(type: "text", nullable: false),
                    Apodo = table.Column<string>(type: "text", nullable: true),
                    RolPrincipal = table.Column<int>(type: "integer", nullable: false),
                    Nacionalidad = table.Column<string>(type: "text", nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Altura = table.Column<int>(type: "integer", nullable: true),
                    Peso = table.Column<int>(type: "integer", nullable: true),
                    ManoHabil = table.Column<string>(type: "text", nullable: true),
                    ImagenPerfilPath = table.Column<string>(type: "text", nullable: true),
                    DuplaId = table.Column<int>(type: "integer", nullable: true),
                    Posicion = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Torneo = table.Column<string>(type: "text", nullable: false),
                    Dupla1Id = table.Column<int>(type: "integer", nullable: false),
                    Dupla2Id = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Clima = table.Column<int>(type: "integer", nullable: false),
                    SetsGanadosDupla1 = table.Column<int>(type: "integer", nullable: false),
                    SetsGanadosDupla2 = table.Column<int>(type: "integer", nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    VideoUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
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
                name: "PartidosEnVivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Torneo = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Dupla1Id = table.Column<int>(type: "integer", nullable: false),
                    Dupla2Id = table.Column<int>(type: "integer", nullable: false)
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
                name: "JugadorLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JugadorId = table.Column<int>(type: "integer", nullable: false),
                    LinkK1 = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    LinkK2 = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    LinkSaque = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    LinkArmado = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    LinkBloqueo = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    LinkExtra = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JugadorLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JugadorLinks_Jugadores_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstadisticaAtaque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartidoId = table.Column<int>(type: "integer", nullable: false),
                    JugadorId = table.Column<int>(type: "integer", nullable: false),
                    Lado = table.Column<int>(type: "integer", nullable: false),
                    Accion = table.Column<int>(type: "integer", nullable: false),
                    Resultado = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    DesdePunto = table.Column<int>(type: "integer", nullable: true),
                    SetNumero = table.Column<int>(type: "integer", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartidoId = table.Column<int>(type: "integer", nullable: false),
                    JugadorId = table.Column<int>(type: "integer", nullable: false),
                    Fuente = table.Column<int>(type: "integer", nullable: false),
                    Resultado = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    DesdePunto = table.Column<int>(type: "integer", nullable: true),
                    Agregados = table.Column<int>(type: "integer", nullable: true),
                    ErroresVarios = table.Column<int>(type: "integer", nullable: true),
                    SetsJugados = table.Column<int>(type: "integer", nullable: true),
                    SetNumero = table.Column<int>(type: "integer", nullable: true),
                    BloqueadoAtqa1 = table.Column<int>(type: "integer", nullable: true),
                    BloqueadoAtqa6 = table.Column<int>(type: "integer", nullable: true),
                    BloqueadoAtqa5 = table.Column<int>(type: "integer", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartidoId = table.Column<int>(type: "integer", nullable: false),
                    JugadorId = table.Column<int>(type: "integer", nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ZonaRecepcion = table.Column<int>(type: "integer", nullable: false),
                    TipoRecepcion = table.Column<int>(type: "integer", nullable: false),
                    TipoSaque = table.Column<int>(type: "integer", nullable: false),
                    ResultadoRecepcion = table.Column<int>(type: "integer", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    DesdePunto = table.Column<int>(type: "integer", nullable: true),
                    SetNumero = table.Column<int>(type: "integer", nullable: true)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartidoId = table.Column<int>(type: "integer", nullable: false),
                    NumeroSet = table.Column<int>(type: "integer", nullable: false),
                    PuntosDupla1 = table.Column<int>(type: "integer", nullable: false),
                    PuntosDupla2 = table.Column<int>(type: "integer", nullable: false),
                    GanadorDuplaId = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartidoId = table.Column<int>(type: "integer", nullable: false),
                    JugadorId = table.Column<int>(type: "integer", nullable: false),
                    LinkK1 = table.Column<string>(type: "text", nullable: true),
                    LinkK2 = table.Column<string>(type: "text", nullable: true),
                    LinkSaque = table.Column<string>(type: "text", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "SetsEnVivo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartidoEnVivoId = table.Column<int>(type: "integer", nullable: false),
                    NumeroSet = table.Column<int>(type: "integer", nullable: false),
                    SacadorInicialDupla1JugadorId = table.Column<int>(type: "integer", nullable: false),
                    SacadorInicialDupla2JugadorId = table.Column<int>(type: "integer", nullable: false),
                    DuplaQueSacaPrimeroId = table.Column<int>(type: "integer", nullable: false),
                    DuplaGanadoraId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetsEnVivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SetsEnVivo_Duplas_DuplaGanadoraId",
                        column: x => x.DuplaGanadoraId,
                        principalTable: "Duplas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SetsEnVivo_Duplas_DuplaQueSacaPrimeroId",
                        column: x => x.DuplaQueSacaPrimeroId,
                        principalTable: "Duplas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SetsEnVivo_Jugadores_SacadorInicialDupla1JugadorId",
                        column: x => x.SacadorInicialDupla1JugadorId,
                        principalTable: "Jugadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SetsEnVivo_Jugadores_SacadorInicialDupla2JugadorId",
                        column: x => x.SacadorInicialDupla2JugadorId,
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SetEnVivoId = table.Column<int>(type: "integer", nullable: false),
                    NumeroRally = table.Column<int>(type: "integer", nullable: false),
                    DuplaGanadoraId = table.Column<int>(type: "integer", nullable: true),
                    MarcadorDupla1 = table.Column<int>(type: "integer", nullable: false),
                    MarcadorDupla2 = table.Column<int>(type: "integer", nullable: false),
                    TipoCierre = table.Column<int>(type: "integer", nullable: true)
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
                    { 9, 2, 4, 3, 2 },
                    { 10, 1, 4, 3, 2 },
                    { 11, 0, 5, 3, 2 },
                    { 12, 5, 1, 4, 2 },
                    { 13, 4, 1, 4, 2 },
                    { 14, 3, 4, 4, 2 },
                    { 15, 2, 4, 4, 2 },
                    { 16, 1, 4, 4, 2 },
                    { 17, 0, 5, 4, 2 }
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
                name: "IX_JugadorLinks_JugadorId",
                table: "JugadorLinks",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "UX_ModificadorCombinada",
                table: "ModificadoresCombinadas",
                columns: new[] { "FundamentoCargado", "CalidadCargada", "FundamentoDerivado" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_Dupla1Id",
                table: "Partidos",
                column: "Dupla1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Partidos_Dupla2Id",
                table: "Partidos",
                column: "Dupla2Id");

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
                name: "IX_Sets_GanadorDuplaId",
                table: "Sets",
                column: "GanadorDuplaId");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_PartidoId",
                table: "Sets",
                column: "PartidoId");

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_DuplaGanadoraId",
                table: "SetsEnVivo",
                column: "DuplaGanadoraId");

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_DuplaQueSacaPrimeroId",
                table: "SetsEnVivo",
                column: "DuplaQueSacaPrimeroId");

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_PartidoEnVivoId",
                table: "SetsEnVivo",
                column: "PartidoEnVivoId");

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_SacadorInicialDupla1JugadorId",
                table: "SetsEnVivo",
                column: "SacadorInicialDupla1JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_SetsEnVivo_SacadorInicialDupla2JugadorId",
                table: "SetsEnVivo",
                column: "SacadorInicialDupla2JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoLinksJugadorPartido_JugadorId",
                table: "VideoLinksJugadorPartido",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoLinksJugadorPartido_PartidoId",
                table: "VideoLinksJugadorPartido",
                column: "PartidoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Acciones_Jugadores_JugadorId",
                table: "Acciones",
                column: "JugadorId",
                principalTable: "Jugadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Acciones_Rallies_RallyId",
                table: "Acciones",
                column: "RallyId",
                principalTable: "Rallies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "DetallesAtaque");

            migrationBuilder.DropTable(
                name: "DetallesRecepcion");

            migrationBuilder.DropTable(
                name: "DetallesSaque");

            migrationBuilder.DropTable(
                name: "EstadisticaAtaque");

            migrationBuilder.DropTable(
                name: "EstadisticaK2");

            migrationBuilder.DropTable(
                name: "EstadisticaRecepcion");

            migrationBuilder.DropTable(
                name: "JugadorLinks");

            migrationBuilder.DropTable(
                name: "ModificadoresCombinadas");

            migrationBuilder.DropTable(
                name: "Sets");

            migrationBuilder.DropTable(
                name: "VideoLinksJugadorPartido");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Acciones");

            migrationBuilder.DropTable(
                name: "Partidos");

            migrationBuilder.DropTable(
                name: "Rallies");

            migrationBuilder.DropTable(
                name: "SetsEnVivo");

            migrationBuilder.DropTable(
                name: "PartidosEnVivo");

            migrationBuilder.DropTable(
                name: "Jugadores");

            migrationBuilder.DropTable(
                name: "Duplas");
        }
    }
}
