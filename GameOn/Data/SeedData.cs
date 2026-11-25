using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.Data
{
    public class SeedData
    {

        // Id du jeu Sudoku dans la table Games 
        public const int SudokuGameId = 1;

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
            modelBuilder.Entity<Models.Game>().HasData(new
            {
                GameId = SudokuGameId,
                Name = "Sudoku",
                Description = "Remplissez la grille de 9x9 avec les chiffres de 1 à 9.",
                Rules = "Chaque ligne, chaque colonne et chaque bloc de 3x3 doit contenir les chiffres de 1 à 9 une seule fois."
            });

            // Accès au Sudoku : départements 1 à 10 (compagnies 1 et 2).
            // Les départements 11 à 15 (compagnie 3) n'y ont pas accès : pour tester le refus.
            modelBuilder.Entity("DepartmentGames").HasData(
                Enumerable.Range(1, 10)
                    .Select(id => new { DepartmentId = id, GameId = SudokuGameId })
                    .ToArray());

        }




    }
}
