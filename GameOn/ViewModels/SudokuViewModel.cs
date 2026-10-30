using GameOn.Commande;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.ViewModels
{
    public class SudokuViewModel : BaseViewModel
    {
        public RelaisCommande RetourCommande { get; }

        public SudokuViewModel(Action retour)
        {
            RetourCommande = new RelaisCommande(retour);
        }
    }
}
