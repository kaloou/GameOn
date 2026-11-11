using GameOn.Data;
using GameOn.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace GameOn.Services
{
    public class AuthService
    {

        private const int LongueurMinMotDePasse = 8;

        // Connexion : renvoie l'employé, ou null si le courriel ou le mot de passe est incorrect
        public async Task<Employee?> SeConnecterAsync(string email, string motDePasse)
        {
            using var context = new AppDbContext();

            Employee? employe = await context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Email == Normaliser(email));

            // Même réponse si le courriel est inconnu ou le mot de passe faux
            if (employe == null || !BCrypt.Net.BCrypt.Verify(motDePasse, employe.Password))
                return null;

            return employe;
        }

        // Inscription trouve la compagnie d'après le domaine du courriel 
        public async Task<Company?> TrouverCompagnieAsync(string email)
        {
            string? domaine = ExtraireDomaine(email);
            if (domaine == null) return null;

            using var context = new AppDbContext();
            return await context.Companies
                .Include(c => c.Departments)
                .FirstOrDefaultAsync(c => c.Domain == domaine);
        }

        // Inscription: crée le compte. Lance une exception avec un message affichable si c'est refusé.
        public async Task<Employee> InscrireAsync(string email, string prenom, string nom,
                                          string motDePasse, int departmentId)
        {
            email = Normaliser(email);

            if (string.IsNullOrWhiteSpace(prenom) || string.IsNullOrWhiteSpace(nom))
                throw new InvalidOperationException("Le prénom et le nom sont obligatoires.");

            if (motDePasse.Length < LongueurMinMotDePasse)
                throw new InvalidOperationException($"Le mot de passe doit contenir au moins {LongueurMinMotDePasse} caractères.");

            using var context = new AppDbContext();

            // Le domaine doit appartenir à une entreprise reconnue
            string? domaine = ExtraireDomaine(email);
            Company? compagnie = await context.Companies
                .Include(c => c.Departments)
                .FirstOrDefaultAsync(c => c.Domain == domaine);

            if (compagnie == null)
                throw new InvalidOperationException("Ce courriel n'appartient à aucune entreprise reconnue.");


            if (!compagnie.Departments.Any(d => d.DepartmentId == departmentId))
                throw new InvalidOperationException("Département invalide pour cette entreprise.");

            if (await context.Employees.AnyAsync(e => e.Email == email))
                throw new InvalidOperationException("Un compte existe déjà avec ce courriel.");

            string hash = BCrypt.Net.BCrypt.HashPassword(motDePasse);

            var employe = new Employee(0, email, prenom.Trim(), nom.Trim(), hash, Role.EMPLOYEE)
            {
                DepartmentId = departmentId
            };

            context.Employees.Add(employe);
            await context.SaveChangesAsync();

            return employe;
        }

        private static string Normaliser(string email) => email.Trim().ToLower();

        private static string? ExtraireDomaine(string email)
        {
            string[] parties = Normaliser(email).Split('@');
            return parties.Length == 2 && parties[0].Length > 0 && parties[1].Contains('.')
                ? parties[1]
                : null;
        }



    }
}
