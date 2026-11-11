using GameOn.Commande;
using GameOn.Models;
using GameOn.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace GameOn.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly AuthService _auth;
        private readonly SessionService _session;
        private readonly Action _connexionReussie;

        private string _email = string.Empty;
        private string _messageErreur = string.Empty;
        private bool _enCours;

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        // Rempli par le code-behind de la vue pour protextion du mot de passe
        public string MotDePasse { get; set; } = string.Empty;

        public string MessageErreur
        {
            get => _messageErreur;
            private set { _messageErreur = value; OnPropertyChanged(); }
        }

        // Vrai pendant la requête à la base : empêche de cliquer deux fois
        public bool EnCours
        {
            get => _enCours;
            private set
            {
                _enCours = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested(); // réévalue le CanExecute du bouton
            }
        }

        public RelaisCommande ConnexionCommande { get; }
        public RelaisCommande InscriptionCommande { get; }

        public LoginViewModel(AuthService auth, SessionService session,
                      Action allerInscription, Action connexionReussie)
        {
            _auth = auth;
            _session = session;
            _connexionReussie = connexionReussie;

            ConnexionCommande = new RelaisCommande(
                async () => await SeConnecterAsync(),
                () => !EnCours && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrEmpty(MotDePasse));

            InscriptionCommande = new RelaisCommande(allerInscription);
        }

        private async Task SeConnecterAsync()
        {
            MessageErreur = string.Empty;
            EnCours = true;

            try
            {
                Employee? employe = await _auth.SeConnecterAsync(Email, MotDePasse);

                if (employe == null)
                {
                    MessageErreur = "Courriel ou mot de passe incorrect.";
                    return;
                }

                _session.Ouvrir(employe);
                Email = string.Empty;
                _connexionReussie();
            }
            catch (Exception)
            {
                // Serveur injoignable: on n'affiche pas le détail technique à l'utilisateur
                MessageErreur = "Connexion à la base de données impossible. Réessayez plus tard.";
            }
            finally
            {
                EnCours = false;
            }
        }
    }
}
