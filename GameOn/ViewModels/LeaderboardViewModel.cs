using GameOn.Commande;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.ViewModels
{
    public class LeaderboardViewModel : BaseViewModel
    {

        public RelaisCommande RetourCommande { get; }

        public LeaderboardViewModel(Action retour)
        {
            RetourCommande = new RelaisCommande(retour);
        }

    }
}
