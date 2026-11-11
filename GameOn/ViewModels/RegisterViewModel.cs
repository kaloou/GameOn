using GameOn.Commande;
using GameOn.Models;
using GameOn.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace GameOn.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly AuthService _auth;
        private readonly SessionService _session;
        private readonly Action _inscriptionReussie;

        private string _email = string.Empty;
        private string _prenom = string.Empty;
        private string _nom = string.Empty;
        private string _nomCompagnie = string.Empty;
        private IReadOnlyList<Department> _departements = new List<Department>();
        private Department? _departementChoisi;
        private string _messageErreur = string.Empty;
        private bool _enCours;

        // Mis à jour quand le champ perd le focus : on cherche alors la compagnie
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                _ = ChargerCompagnieAsync(value);
            }
        }

        public string Prenom
        {
            get => _prenom;
            set { _prenom = value; OnPropertyChanged(); }
        }

        public string Nom
        {
            get => _nom;
            set { _nom = value; OnPropertyChanged(); }
        }

        public string MotDePasse { get; set; } = string.Empty;
        public string ConfirmationMotDePasse { get; set; } = string.Empty;


        public string NomCompagnie
        {
            get => _nomCompagnie;
            private set { _nomCompagnie = value; OnPropertyChanged(); }
        }

        public IReadOnlyList<Department> Departements
        {
            get => _departements;
            private set { _departements = value; OnPropertyChanged(); }
        }

        public Department? DepartementChoisi
        {
            get => _departementChoisi;
            set { _departementChoisi = value; OnPropertyChanged(); }
        }

        public string MessageErreur
        {
            get => _messageErreur;
            private set { _messageErreur = value; OnPropertyChanged(); }
        }

        public bool EnCours
        {
            get => _enCours;
            private set
            {
                _enCours = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public RelaisCommande InscrireCommande { get; }
        public RelaisCommande RetourCommande { get; }

        public RegisterViewModel(AuthService auth, SessionService session,
                         Action retour, Action inscriptionReussie)
        {
            _auth = auth;
            _session = session;
            _inscriptionReussie = inscriptionReussie;

            InscrireCommande = new RelaisCommande(
                async () => await InscrireAsync(),
                () => !EnCours
                      && DepartementChoisi != null
                      && !string.IsNullOrWhiteSpace(Prenom)
                      && !string.IsNullOrWhiteSpace(Nom)
                      && !string.IsNullOrEmpty(MotDePasse));

            RetourCommande = new RelaisCommande(retour);
        }

        private async Task ChargerCompagnieAsync(string email)
        {
            NomCompagnie = string.Empty;
            Departements = new List<Department>();
            DepartementChoisi = null;
            MessageErreur = string.Empty;

            if (string.IsNullOrWhiteSpace(email)) return;

            try
            {
                Company? compagnie = await _auth.TrouverCompagnieAsync(email);

                // L'utilisateur a pu modifier le courriel pendant la recherche : on ignore l'ancien résultat
                if (email != Email) return;

                if (compagnie == null)
                {
                    MessageErreur = "Ce courriel n'appartient à aucune entreprise reconnue.";
                    return;
                }

                NomCompagnie = $"Entreprise : {compagnie.Name}";
                Departements = compagnie.Departments;
            }
            catch (Exception)
            {
                MessageErreur = "Connexion à la base de données impossible. Réessayez plus tard.";
            }
        }

        private async Task InscrireAsync()
        {
            MessageErreur = string.Empty;

            if (MotDePasse != ConfirmationMotDePasse)
            {
                MessageErreur = "Les mots de passe ne correspondent pas.";
                return;
            }

            EnCours = true;
            try
            {
                Employee employe = await _auth.InscrireAsync(
                    Email, Prenom, Nom, MotDePasse, DepartementChoisi!.DepartmentId);

                _session.Ouvrir(employe);
                _inscriptionReussie();
            }
            catch (InvalidOperationException ex)
            {
                // Refus prévus par AuthService (courriel déjà utilisé, mot de passe trop court...)
                MessageErreur = ex.Message;
            }
            catch (Exception)
            {
                MessageErreur = "Connexion à la base de données impossible. Réessayez plus tard.";
            }
            finally
            {
                EnCours = false;
            }
        }

    }
}
