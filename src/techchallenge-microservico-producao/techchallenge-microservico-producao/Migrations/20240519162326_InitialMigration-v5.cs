using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace techchallenge_microservico_producao.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrationv5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdUsuarioOrigem",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdUsuarioOrigem",
                table: "Usuarios");
        }
    }
}
