using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameOn.Migrations
{
    /// <summary>
    /// GameData ne garde que la progression (Valeurs + Notes) :
    /// la grille de départ et la solution sont déjà dans le DailyChallenge de la partie.
    /// </summary>
    public partial class SlimSudokuGameData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seulement les parties liées à un défi du jour : pour les anciennes parties (sans défi),
            // GameData est la seule copie de leur grille, on la conserve
            migrationBuilder.Sql(@"
                UPDATE GameSessions
                SET GameData = JSON_REMOVE(GameData, '$.GrilleDepart', '$.Solution')
                WHERE DailyChallengeId IS NOT NULL
                  AND JSON_VALID(GameData);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ponytail: pas de retour arrière des données, la grille et la solution restent dans DailyChallenges.
            // Reconstruire GrilleDepart/Solution en SQL seulement si on doit vraiment revenir à l'ancien format.
        }
    }
}
