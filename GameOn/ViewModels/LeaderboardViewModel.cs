using GameOn.Commande;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.ViewModels
{
    // Une ligne du classement
    public record LigneClassement(int Rang, string Initiales, string Nom, string Departement, string Meilleur, int Points);

    public class LeaderboardViewModel : BaseViewModel
    {

        public RelaisCommande RetourCommande { get; }

        // ponytail: données d'exemple de la maquette, à remplacer par les GameScore quand le calcul des scores existera
        public List<LigneClassement> Lignes { get; } = new()
        {
            new(1, "ÉT", "Émilie Tremblay", "Développement", "8:34", 45),
            new(2, "SC", "Sarah Côté", "Développement", "8:26", 28),
            new(3, "JB", "Julien Bouchard", "Assurance qualité", "8:09", 24),
            new(4, "NP", "Noémie Pelletier", "Analyse d'affaires", "10:18", 20),
            new(5, "MA", "Marc-Antoine Gagnon", "Gestion de projet", "9:55", 6),
            new(6, "OF", "Olivier Fortin", "Développement", "10:36", 6),
            new(7, "CR", "Camille Roy", "Analyse d'applications", "13:07", 6),
            new(8, "MM", "Mathieu Morin", "Analyse d'applications", "14:04", 3),
            new(9, "LG", "Léa Gauthier", "Gestion de projet", "11:35", 1),
            new(10, "SG", "Samuel Girard", "Développement", "13:11", 1),
            new(11, "KL", "Kevin Lavoie", "Assurance qualité", "13:05", 0),
            new(12, "AB", "Audrey Bélanger", "Analyse d'affaires", "18:01", 0),
        };

        public LeaderboardViewModel(Action retour)
        {
            RetourCommande = new RelaisCommande(retour);
        }

    }
}
