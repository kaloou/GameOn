using DotNetEnv;
using GameOn.Models;
using GameOn.Models.Sudoku;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GameOn.Data
{
    public class AppDbContext : DbContext
    {
        // Une table par DbSet
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Game> Games => Set<Game>();
        public DbSet<Sudoku> Sudokus => Set<Sudoku>();
        public DbSet<GameSession> GameSessions => Set<GameSession>();
        public DbSet<GameScore> GameScores => Set<GameScore>();
        public DbSet<DailyChallenge> DailyChallenges => Set<DailyChallenge>();

        // évite une requête à chaque création du contexte
        private static ServerVersion? _versionServeur;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            // Nécessaire aussi pour les commandes de migration, qui ne passent pas par App.xaml.cs
            Env.TraversePath().Load();

            string connexion = Environment.GetEnvironmentVariable("DB_CONNECTION")
                ?? throw new InvalidOperationException("DB_CONNECTION est absent du fichier .env");

            _versionServeur ??= ServerVersion.AutoDetect(connexion);
            optionsBuilder.UseMySql(connexion, _versionServeur);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Les enums sont stockés en texte ("ADMIN", "EASY"...) plutôt qu'en nombre : plus lisible en base
            configurationBuilder.Properties<Role>().HaveConversion<string>();
            configurationBuilder.Properties<Difficulty>().HaveConversion<string>();
            configurationBuilder.Properties<Status>().HaveConversion<string>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Clés primaires dont le nom ne suit pas la convention (Id ou NomDeLaClasseId)
            modelBuilder.Entity<GameSession>().HasKey(s => s.SessionId);
            modelBuilder.Entity<GameScore>().HasKey(s => s.ScoreId);
            modelBuilder.Entity<DailyChallenge>().HasKey(c => c.ChallengeId);

            // Email unique (une longueur max est obligatoire pour un index MySQL)
            modelBuilder.Entity<Employee>(e =>
            {
                e.Property(x => x.Email).HasMaxLength(255);
                e.HasIndex(x => x.Email).IsUnique();
            });

            modelBuilder.Entity<Company>(c =>
            {
                c.Property(x => x.Domain).HasMaxLength(255);
                c.HasIndex(x => x.Domain).IsUnique();
            });

            // Point 4 : Preferences (List<int>) stockée en texte "1,3,5"
            modelBuilder.Entity<Employee>()
                .Property(e => e.Preferences)
                .HasConversion(
                    liste => string.Join(',', liste),
                    texte => texte.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(s => int.Parse(s))
                                  .ToList(),
                    new ValueComparer<List<int>>(
                        (a, b) => a!.SequenceEqual(b!),
                        l => l.Aggregate(0, (h, v) => HashCode.Combine(h, v)),
                        l => l.ToList()));

            // Le sujet exige de conserver toutes les données :
            // supprimer une ligne ne doit jamais effacer en cascade l'historique lié
            foreach (var cle in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()))
            {
                cle.DeleteBehavior = DeleteBehavior.Restrict;
            }

            SeedData.Appliquer(modelBuilder);

        }
    }
}