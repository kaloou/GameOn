using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.Models
{
    public class GameScore
    {
        private int scoreId;
        public int ScoreId {
            get{ return scoreId;}
            set{ scoreId = value;} 
        }
        private int weekNb;

		public int WeekNb
		{
			get { return weekNb; }
			set { weekNb = value; }
		}

        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

		private int points;

		public int Points
		{
			get { return points; }
			set { points = value; }
		}

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public int GameId { get; set; }
        public Game Game { get; set; } = null!;

        public int GameSessionId { get; set; }
        public GameSession GameSession { get; set; } = null!;

        public GameScore(int scoreId, int weekNb, DateTime date, int points)
        {
            ScoreId = scoreId;
            WeekNb = weekNb;
            Date = date;
            Points = points;
        }



    }
}
