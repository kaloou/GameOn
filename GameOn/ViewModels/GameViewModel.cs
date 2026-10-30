using GameOn.Commande;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.ViewModels
{
    public class GameViewModel : BaseViewModel
    {

        public RelaisCommande RetourCommande { get; }
        public RelaisCommande JouerCommande { get; }

        public GameViewModel(Action retour, Action jouer)
        {
            RetourCommande = new RelaisCommande(retour);
            JouerCommande = new RelaisCommande(jouer);

        }
    }
}
