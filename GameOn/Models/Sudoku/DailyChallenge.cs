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

        // Une grille différente par difficulté (casual compris)
        public Difficulty Difficulty { get; set; }

        public List<GameSession> GameSessions { get; set; } = new();

        public DailyChallenge(int challengeId, DateTime date, string challengeData, Difficulty difficulty)
        {
            ChallengeId = challengeId;
            Date = date;
            ChallengeData = challengeData;
            Difficulty = difficulty;
        }
    }
}
