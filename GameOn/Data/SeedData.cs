using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.Data
{
    public class SeedData
    {
        private static readonly string[] NomsDepartements =
        {
            "Gestion de projet",
            "Analyse d'affaires",
            "Analyse d'applications",
            "Développement",
            "Assurance qualité"
        };

        public static void Appliquer(ModelBuilder modelBuilder)
        {
            // Compagnies pour le test, avec des domaines fictifs
            var compagnies = new[]
            {
                new { CompanyId = 1, Name = "Hourglass Unlimited", Domain = "hourglass.test" },
                new { CompanyId = 2, Name = "Mathieu Tech", Domain = "math.test" },
                new { CompanyId = 3, Name = "edgardCoorp Tech", Domain = "bbk.test" }
            };
            modelBuilder.Entity<Models.Company>().HasData(compagnies);

            // Chaque compagnie reçoit les 5 départements
            var departements = new List<object>();
            int id = 1;
            foreach (var compagnie in compagnies)
            {
                foreach (string nom in NomsDepartements)
                {
                    departements.Add(new { DepartmentId = id++, Name = nom, CompanyId = compagnie.CompanyId });
                }
            }
            modelBuilder.Entity<Models.Department>().HasData(departements);

            // Le jeu Sudoku : chaque GameSession doit être reliée à un jeu
            modelBuilder.Entity<Models.Sudoku.Sudoku>().HasData(new
            {
                GameId = 1,
                Name = "Sudoku",
                Description = "Remplissez la grille de 9x9 avec les chiffres de 1 à 9.",
                Rules = "Chaque ligne, chaque colonne et chaque bloc de 3x3 doit contenir les chiffres de 1 à 9 une seule fois.",
                BeginTime = 0,
                EndTime = 0,
                Difficulty = Models.Difficulty.EASY
            });

        }




    }
}
