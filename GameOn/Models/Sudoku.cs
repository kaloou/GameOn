using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.Models
{
    internal class Sudoku : Game
    {
        private int beginTime { get; set; }
        private int endTime { get; set; }

        public int BeginTime
        {
            get => beginTime;
            set => beginTime = value;
        }

        public int EndTime
        {
            get => endTime;
            set => endTime = value;
        }

        public Sudoku(int beginTime, int endTime, int gameId, string name, string description, string rules) : base(gameId, name, description, rules)
        {
            BeginTime = beginTime;
            EndTime = endTime;
        }
    }
}
