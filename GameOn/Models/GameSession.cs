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
        public Difficulty Difficulty { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CompletedAt { get; set; }
        public Status Status { get; set; }

        public int EmployeeId { get; set; }

        public List<GameScore> GameScores { get; set; } = new();

        public GameSession(int sessionId, string gameData,
                   Difficulty difficulty, DateTime startedAt,
                   Status status)
        {
            SessionId = sessionId;
            GameData = gameData;
            Difficulty = difficulty;
            StartedAt = startedAt;
            Status = status;
        }

    }
}
