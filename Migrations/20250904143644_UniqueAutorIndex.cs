using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagerPro.Migrations
{
    /// <inheritdoc />
    public partial class UniqueAutorIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Autores_Nome_DataNascimento",
                table: "Autores",
                columns: new[] { "Nome", "DataNascimento" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Autores_Nome_DataNascimento",
                table: "Autores");
        }
    }
}
