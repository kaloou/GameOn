using GameOn.Models;
using GameOn.Models.Sudoku;

namespace GameOn.Services
{
    // ChallengeData = réponse JSON de l'API Sudoku, avec "puzzle" et "solution".
    public interface IDailyChallengeProvider
    {
        // Défi du jour pour cette difficulté (EASY, MEDIUM ou HARD), créé s'il n'existe pas
        Task<DailyChallenge> ObtenirDefiDuJourAsync(Difficulty difficulty);
    }
}