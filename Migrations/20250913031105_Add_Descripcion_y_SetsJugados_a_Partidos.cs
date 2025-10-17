using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SandStats.Migrations
{
    /// <inheritdoc />
    public partial class Add_Descripcion_y_SetsJugados_a_Partidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Partidos",
                type: "character varying(256)", // el tamaño que quieras
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetsJugados",
                table: "Partidos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
