using GameOn.Games.Sudoku;
using GameOn.Models;
using GameOn.Models.Sudoku;

namespace GameOn.Services
{
    // TEMP : à remplacer par le service de la personne 1.
    // Une grille de l'API, non enregistrée en base (ChallengeId = 0).
    public class DefiTemporaireProvider : IDailyChallengeProvider
    {
        public async Task<DailyChallenge> ObtenirDefiDuJourAsync(Difficulty difficulty)
        {
            string json = await SudokuApi.GetSudokuPuzzleAsync(difficulty);
            return new DailyChallenge(0, DateTime.Today, json);
        }
    }
}