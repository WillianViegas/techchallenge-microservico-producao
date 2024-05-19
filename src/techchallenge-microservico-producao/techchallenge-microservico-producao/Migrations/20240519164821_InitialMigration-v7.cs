using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace techchallenge_microservico_producao.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrationv7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdPagamentoOrigem",
                table: "Pagamentos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdPagamentoOrigem",
                table: "Pagamentos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
