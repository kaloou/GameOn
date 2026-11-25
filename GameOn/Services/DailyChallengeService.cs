using GameOn.Data;
using GameOn.Games.Sudoku;
using GameOn.Models;
using GameOn.Models.Sudoku;
using Microsoft.EntityFrameworkCore;

namespace GameOn.Services
{
    // Défis du jour : un par date et par difficulté (casual compris), enregistrés en base
    public class DailyChallengeService : IDailyChallengeProvider
    {
        public async Task<DailyChallenge> ObtenirDefiDuJourAsync(Difficulty difficulty)
        {
            return await TrouverAsync(DateTime.Today, difficulty)
                ?? await GenererAsync(DateTime.Today, difficulty);
        }

        private static async Task<DailyChallenge?> TrouverAsync(DateTime date, Difficulty difficulty)
        {
            using var context = new AppDbContext();
            return await context.DailyChallenges
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Date == date && c.Difficulty == difficulty);
        }

        // Génère et enregistre le défi. Si une autre exécution l'a enregistré entre-temps,
        // l'index unique (Date, Difficulty) refuse le doublon : on renvoie celui déjà en base
        private static async Task<DailyChallenge> GenererAsync(DateTime date, Difficulty difficulty)
        {
            string json = await SudokuApi.GetSudokuPuzzleAsync(difficulty, date);
            var defi = new DailyChallenge(0, date, json, difficulty);

            try
            {
                using var context = new AppDbContext();
                context.DailyChallenges.Add(defi);
                await context.SaveChangesAsync();
                return defi;
            }
            catch (DbUpdateException)
            {
                DailyChallenge? existant = await TrouverAsync(date, difficulty);
                if (existant == null) throw;
                return existant;
            }
        }
    }
}
