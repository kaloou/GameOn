using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameOn.Migrations
{
    /// <inheritdoc />
    public partial class SeedSudokuGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "GameId", "BeginTime", "Description", "Difficulty", "Discriminator", "EndTime", "Name", "Rules" },
                values: new object[] { 1, 0, "Remplissez la grille de 9x9 avec les chiffres de 1 à 9.", "EASY", "Sudoku", 0, "Sudoku", "Chaque ligne, chaque colonne et chaque bloc de 3x3 doit contenir les chiffres de 1 à 9 une seule fois." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "GameId",
                keyValue: 1);
        }
    }
}
