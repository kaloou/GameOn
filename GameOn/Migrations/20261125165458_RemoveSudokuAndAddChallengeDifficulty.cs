using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameOn.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSudokuAndAddChallengeDifficulty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyChallenges_Games_SudokuId",
                table: "DailyChallenges");

            migrationBuilder.DropIndex(
                name: "IX_DailyChallenges_SudokuId",
                table: "DailyChallenges");

            migrationBuilder.DropColumn(
                name: "BeginTime",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "SudokuId",
                table: "DailyChallenges");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Difficulty",
                table: "DailyChallenges",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DailyChallenges_Date_Difficulty",
                table: "DailyChallenges",
                columns: new[] { "Date", "Difficulty" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyChallenges_Date_Difficulty",
                table: "DailyChallenges");

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "GameId",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "DailyChallenges");

            migrationBuilder.AddColumn<int>(
                name: "BeginTime",
                table: "Games",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Difficulty",
                table: "Games",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Games",
                type: "varchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "EndTime",
                table: "Games",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Employees",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SudokuId",
                table: "DailyChallenges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "GameId", "BeginTime", "Description", "Difficulty", "Discriminator", "EndTime", "Name", "Rules" },
                values: new object[] { 1, 0, "Remplissez la grille de 9x9 avec les chiffres de 1 à 9.", "EASY", "Sudoku", 0, "Sudoku", "Chaque ligne, chaque colonne et chaque bloc de 3x3 doit contenir les chiffres de 1 à 9 une seule fois." });

            migrationBuilder.CreateIndex(
                name: "IX_DailyChallenges_SudokuId",
                table: "DailyChallenges",
                column: "SudokuId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyChallenges_Games_SudokuId",
                table: "DailyChallenges",
                column: "SudokuId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
