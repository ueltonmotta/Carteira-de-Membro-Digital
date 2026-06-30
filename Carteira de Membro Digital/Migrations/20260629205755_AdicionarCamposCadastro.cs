using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Carteira_de_Membro_Digital.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCamposCadastro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Usuarios");
        }
    }
}
