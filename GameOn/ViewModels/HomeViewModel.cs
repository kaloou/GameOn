using GameOn.Commande;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public RelaisCommande VoirJeuxCommande { get; }
        public RelaisCommande JouerCommande { get; }
        public RelaisCommande ClassementCommande { get; }

        public HomeViewModel(Action voirJeux, Action jouer, Action voirClassement)
        {
            VoirJeuxCommande = new RelaisCommande(voirJeux);
            JouerCommande = new RelaisCommande(jouer);
            ClassementCommande = new RelaisCommande(voirClassement);
        }
    }
}
