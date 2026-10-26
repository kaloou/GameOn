using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.Models
{
    internal class Game
    {
        private int gameId;
        private string name = string.Empty;
        private string description = string.Empty;
        private string rules = string.Empty;

        public int GameId {
            get => gameId; 
            set => gameId = value; 
        }

        public string Name {
            get => name;
            set => name = !string.IsNullOrEmpty(value)
                ? value 
                : throw new ArgumentException("Name cannot be null or empty");
        }

        public string Description
        {
            get => description;
            set => description = !string.IsNullOrEmpty(value)
                ? value
                : throw new ArgumentException("Description cannot be null or empty");
        }
        public string Rules
        {
            get => rules;
            set => rules = !string.IsNullOrEmpty(value)
                ? value
                : throw new ArgumentException("Rules cannot be null or empty");
        }
        public Game(int gameId, string name, string description, string rules)
        {
            GameId = gameId;
            Name = name;
            Description = description;
            Rules = rules;
        }
    }
}
