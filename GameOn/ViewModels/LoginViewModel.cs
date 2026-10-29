using GameOn.Commande;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public RelaisCommande InscriptionCommande { get; }
        public RelaisCommande ConnexionCommande { get; }

        public LoginViewModel(Action allerInscription, Action seConnecter)
        {
            InscriptionCommande = new RelaisCommande(allerInscription);
            ConnexionCommande = new RelaisCommande(seConnecter);
        }
    }
}
