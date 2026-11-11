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
        }




    }
}
