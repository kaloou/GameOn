using GameOn.ViewModels;
using GameOn.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.Services
{
    public class SessionService : BaseViewModel
    {

        private Employee? _utilisateur;

        public Employee? Utilisateur
        {
            get => _utilisateur;
            private set
            {
                _utilisateur = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EstConnecte));
                OnPropertyChanged(nameof(EstDeconnecte));
                OnPropertyChanged(nameof(NomAffiche));
            }
        }

        public bool EstConnecte => Utilisateur != null;
        public bool EstDeconnecte => Utilisateur == null;
        public string NomAffiche => Utilisateur == null ? "" : $"{Utilisateur.Firstname} {Utilisateur.Lastname}";

        public void Ouvrir(Employee employe) => Utilisateur = employe;
        public void Fermer() => Utilisateur = null;
    }
}
