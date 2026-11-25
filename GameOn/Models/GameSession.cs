using GameOn.Models.Sudoku;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GameOn.Models
{
    public class GameSession
    {
        public int SessionId { get; set; }
        public string GameData { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Status Status { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public int GameId { get; set; }
        public Game Game { get; set; } = null!;

        // La difficulté de la partie est celle de son défi du jour
        // (nullable : les parties créées avant les défis en base n'en ont pas)
        public int? DailyChallengeId { get; set; }
        public DailyChallenge? DailyChallenge { get; set; }

        public GameScore? GameScore { get; set; }

        public GameSession(int sessionId, string gameData, DateTime startedAt, Status status)
        {
            SessionId = sessionId;
            GameData = gameData;
            StartedAt = startedAt;
            Status = status;
        }

    }
}
