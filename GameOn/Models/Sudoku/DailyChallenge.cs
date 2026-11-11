using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.Models.Sudoku
{
    public class DailyChallenge
    {
        private int challengeId;
        private DateTime date;
        private string challengeData = string.Empty;
        // private Difficulty difficulty;

        public int ChallengeId
        {
            get => challengeId;
            set => challengeId = value;
        }
        public DateTime Date
        {
            get => date;
            set => date = value;
        }
        public string ChallengeData
        {
            get => challengeData;
            set => challengeData = !string.IsNullOrEmpty(value)
                ? value
                : throw new ArgumentException("ChallengeData cannot be null or empty");
        }

        // Le Sudoku du défi
        public int SudokuId { get; set; }
        public Sudoku Sudoku { get; set; } = null!;

        public List<GameSession> GameSessions { get; set; } = new();

        public DailyChallenge(int challengeId, DateTime date, string challengeData)
        {
            ChallengeId = challengeId;
            Date = date;
            ChallengeData = challengeData;
        }
    }
}
