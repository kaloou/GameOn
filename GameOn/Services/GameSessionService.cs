using GameOn.Data;
using GameOn.Models;
using Microsoft.EntityFrameworkCore;

namespace GameOn.Services
{
    public class GameSessionService
    {

        // La partie du joueur sur le défi du jour de cette difficulté (la difficulté vient du DailyChallenge)
        public async Task<GameSession?> TrouverPartieDuJourAsync(int employeeId, int gameId, Difficulty difficulty)
        {
            using var context = new AppDbContext();
            return await context.GameSessions
                .AsNoTracking()
                .Include(s => s.DailyChallenge) // grille de départ et solution, pour reprendre la partie
                .Where(s => s.EmployeeId == employeeId
                         && s.GameId == gameId
                         && s.DailyChallenge!.Difficulty == difficulty
                         && s.DailyChallenge.Date == DateTime.Today
                         && s.Status != Status.EXPIRED) // une partie terminée est renvoyée aussi : pas de 2e partie le même jour
                .OrderByDescending(s => s.StartedAt)
                .FirstOrDefaultAsync();
        }

        // Enregistre l'état actuel 
        public async Task SauvegarderAsync(int sessionId, string gameData)
        {
            using var context = new AppDbContext();
            await context.GameSessions
                .Where(s => s.SessionId == sessionId && s.Status == Status.STARTED)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.GameData, gameData));
        }

        // Termine la partie : état final + date de fin. Renvoie la date de fin, ou null si la partie n'était plus en cours
        public async Task<DateTime?> TerminerAsync(int sessionId, string gameData)
        {
            DateTime fin = DateTime.Now;

            using var context = new AppDbContext();
            int lignes = await context.GameSessions
                .Where(s => s.SessionId == sessionId && s.Status == Status.STARTED)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.GameData, gameData)
                    .SetProperty(x => x.Status, Status.COMPLETED)
                    .SetProperty(x => x.CompletedAt, fin));

            return lignes == 1 ? fin : null;
        }

        // Le jeu est-il accessible au département du joueur ?
        public async Task<bool> EstAccessibleAsync(int departmentId, int gameId)
        {
            using var context = new AppDbContext();
            return await context.Departments
                .AnyAsync(d => d.DepartmentId == departmentId
                            && d.Games.Any(g => g.GameId == gameId));
        }

        // Nouvelle partie sur un défi du jour, avec l'état de départ de la grille
        public async Task<GameSession> CreerPartieAsync(int employeeId, int gameId, int dailyChallengeId, string gameData)
        {
            var session = new GameSession(0, gameData, DateTime.Now, Status.STARTED)
            {
                EmployeeId = employeeId,
                GameId = gameId,
                DailyChallengeId = dailyChallengeId
            };

            using var context = new AppDbContext();
            context.GameSessions.Add(session);
            await context.SaveChangesAsync();

            return session; // SessionId est rempli par MySQL après SaveChanges
        }

    }
}