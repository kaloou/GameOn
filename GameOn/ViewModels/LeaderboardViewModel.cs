using GameOn.Commande;
using GameOn.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.ViewModels
{
    // Une ligne du classement
    public record LigneClassement(int Rang, string Initiales, string Nom, string Departement, string Meilleur, int Points);

    public class LeaderboardViewModel : BaseViewModel
    {
        private const string TousLesDepartements = "Tous les départements";

        // Libellé affiché -> texte du sous-titre
        private static readonly Dictionary<string, string> Periodes = new()
        {
            ["Today"] = "aujourd'hui",
            ["This week"] = "cette semaine",
            ["Last week"] = "la semaine dernière",
            ["All time"] = "depuis le début"
        };

        // ponytail: données d'exemple de la maquette, à remplacer par les GameScore quand le calcul des scores existera
        private static readonly List<LigneClassement> Exemple = new()
        {
            new(0, "ÉT", "Émilie Tremblay", "Développement", "8:34", 45),
            new(0, "SC", "Sarah Côté", "Développement", "8:26", 28),
            new(0, "JB", "Julien Bouchard", "Assurance qualité", "8:09", 24),
            new(0, "NP", "Noémie Pelletier", "Analyse d'affaires", "10:18", 20),
            new(0, "MA", "Marc-Antoine Gagnon", "Gestion de projet", "9:55", 6),
            new(0, "OF", "Olivier Fortin", "Développement", "10:36", 6),
            new(0, "CR", "Camille Roy", "Analyse d'applications", "13:07", 6),
            new(0, "MM", "Mathieu Morin", "Analyse d'applications", "14:04", 3),
            new(0, "LG", "Léa Gauthier", "Gestion de projet", "11:35", 1),
            new(0, "SG", "Samuel Girard", "Développement", "13:11", 1),
            new(0, "KL", "Kevin Lavoie", "Assurance qualité", "13:05", 0),
            new(0, "AB", "Audrey Bélanger", "Analyse d'affaires", "18:01", 0),
        };

        public RelaisCommande RetourCommande { get; }

        // ===== Filtres =====
        // (value ?? ancien : un Ctrl+clic peut désélectionner un choix, on garde alors le filtre actuel)
        public List<string> Jeux { get; } = new() { "Sudoku" };
        public List<string> Departements { get; } = new List<string> { TousLesDepartements }.Concat(SeedData.NomsDepartements).ToList();
        public List<string> Difficultes { get; } = new() { "Easy", "Normal", "Hard" };
        public List<string> PeriodesAffichees { get; } = Periodes.Keys.ToList();

        private string jeu = "Sudoku";
        public string Jeu
        {
            get => jeu;
            set { jeu = value ?? jeu; OnAllPropertiesChanged(); }
        }

        private string departement = TousLesDepartements;
        public string Departement
        {
            get => departement;
            set { departement = value ?? departement; OnAllPropertiesChanged(); }
        }

        private string difficulte = "Hard";
        public string Difficulte
        {
            get => difficulte;
            set { difficulte = value ?? difficulte; OnAllPropertiesChanged(); }
        }

        private string periode = "Last week";
        public string Periode
        {
            get => periode;
            set { periode = value ?? periode; OnAllPropertiesChanged(); }
        }

        // Ex. : "Sudoku · Hard · la semaine dernière"
        public string SousTitre => $"{Jeu} · {Difficulte} · {Periodes[Periode]}";

        // Classement filtré, rangs recalculés
        // ponytail: seul le département filtre les données d'exemple ; jeu, difficulté et période filtreront les vrais scores
        public List<LigneClassement> Lignes =>
            Exemple.Where(l => Departement == TousLesDepartements || l.Departement == Departement)
                   .Select((l, i) => l with { Rang = i + 1 })
                   .ToList();

        public LeaderboardViewModel(Action retour)
        {
            RetourCommande = new RelaisCommande(retour);
        }

    }
}
