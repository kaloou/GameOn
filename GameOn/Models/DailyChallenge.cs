using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.Models
{
    internal class DailyChallenge
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

        public DailyChallenge(int challengeId, DateTime date, string challengeData)
        {
            ChallengeId = challengeId;
            Date = date;
            ChallengeData = challengeData;
        }
    }
}
