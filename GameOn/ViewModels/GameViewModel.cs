using GameOn.Commande;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.ViewModels
{
    public class GameViewModel : BaseViewModel
    {

        public RelaisCommande RetourCommande { get; }

        public GameViewModel(Action retour)
        {
            RetourCommande = new RelaisCommande(retour);
        }

    }
}
