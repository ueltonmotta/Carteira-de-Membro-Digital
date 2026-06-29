using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carteira_de_Membro_Digital.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarPresenca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventoId",
                table: "Presencas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Motivo",
                table: "Presencas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Presencas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventoId",
                table: "Presencas");

            migrationBuilder.DropColumn(
                name: "Motivo",
                table: "Presencas");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Presencas");
        }
    }
}
