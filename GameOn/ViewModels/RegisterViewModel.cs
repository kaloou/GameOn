using GameOn.Commande;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        public RelaisCommande RetourCommande { get; }

        public RegisterViewModel(Action retour)
        {
            RetourCommande = new RelaisCommande(retour);
        }
    }
}
