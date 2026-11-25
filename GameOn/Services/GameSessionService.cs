using GameOn.Data;
using GameOn.Models;
using Microsoft.EntityFrameworkCore;

namespace GameOn.Services
{
    public class GameSessionService
    {

        public async Task<GameSession?> TrouverPartieDuJourAsync(int employeeId, int gameId, Difficulty difficulty)
        {
            DateTime debutJournee = DateTime.Today;
            DateTime finJournee = debutJournee.AddDays(1);

            using var context = new AppDbContext();
            return await context.GameSessions
                .AsNoTracking()
                .Where(s => s.EmployeeId == employeeId
                         && s.GameId == gameId
                         && s.Difficulty == difficulty
                         && s.Status != Status.EXPIRED // une partie terminée est renvoyée aussi : pas de 2e partie le même jour
                         && s.StartedAt >= debutJournee
                         && s.StartedAt < finJournee)
                .OrderByDescending(s => s.StartedAt)
                .FirstOrDefaultAsync();
        }

        // Nouvelle partie, avec l'état de départ de la grille
        public async Task<GameSession> CreerPartieAsync(int employeeId, int gameId, Difficulty difficulty, string gameData)
        {
            var session = new GameSession(0, gameData, difficulty, DateTime.Now, Status.STARTED)
            {
                EmployeeId = employeeId,
                GameId = gameId
            };

            using var context = new AppDbContext();
            context.GameSessions.Add(session);
            await context.SaveChangesAsync();

            return session; // SessionId est rempli par MySQL après SaveChanges
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

        // Le jeu est-il accessible au département du joueur ? (pas de département = pas d'accès)
        public async Task<bool> EstAccessibleAsync(int? departmentId, int gameId)
        {
            if (departmentId == null) return false;

            using var context = new AppDbContext();
            return await context.Departments
                .AnyAsync(d => d.DepartmentId == departmentId
                            && d.Games.Any(g => g.GameId == gameId));
        }

    }
}