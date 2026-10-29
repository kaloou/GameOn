using GameOn.Commande;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public RelaisCommande JouerCommande { get; }
        public RelaisCommande ClassementCommande { get; }
        public RelaisCommande DeconnexionCommande { get; }

        public HomeViewModel(Action jouer, Action classement, Action deconnexion)
        {
            JouerCommande = new RelaisCommande(jouer);
            ClassementCommande = new RelaisCommande(classement);
            DeconnexionCommande = new RelaisCommande(deconnexion);
        }
    }
}
