using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GameOn.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Role Role { get; set; }

        public List<int> Preferences { get; set; } = new();

        public List<GameSession> GameSessions { get; set; } = new();
        public List<GameScore> GameScores { get; set; } = new();

        public Employee(int employeeId, string email, string firstname, string lastname, string password, Role role)
        {
            EmployeeId = employeeId;
            Email = email;
            Firstname = firstname;
            Lastname = lastname;
            Password = password;
            Role = role;
            Preferences = new List<int>();
        }
    }
}
