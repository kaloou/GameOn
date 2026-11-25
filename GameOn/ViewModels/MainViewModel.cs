using GameOn.Commande;
using GameOn.Data;
using GameOn.Models.Sudoku;
using GameOn.Services;
using GameOn.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GameOn.ViewModels
{
    public class MainViewModel : BaseViewModel
    {

        private readonly AuthService _auth = new AuthService();
        private readonly GameSessionService _sessions = new GameSessionService();

        private BaseViewModel _currentViewModel;

        public LoginViewModel LoginVM { get; }
        public RegisterViewModel RegisterVM { get; }
        public HomeViewModel HomeVM { get; }
        public GameViewModel GameVM { get; }
        public LeaderboardViewModel LeaderboardVM { get; }
        public SudokuViewModel? SudokuVM { get; private set; }

        public SessionService Session { get; } = new SessionService();

        public RelaisCommande DeconnexionCommande { get; }
        public RelaisCommande AccueilCommande { get; }
        public RelaisCommande JeuxCommande { get; }
        public RelaisCommande ClassementCommande { get; }
        public RelaisCommande ConnexionCommande { get; }

        public RelaisCommande SudokuCommande { get; }

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (value != null)
                {
                    _currentViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainViewModel()
        {

            LoginVM = new LoginViewModel(
                        _auth,
                        Session,
                        allerInscription: () => CurrentViewModel = RegisterVM,
                        connexionReussie: () => CurrentViewModel = HomeVM);

            RegisterVM = new RegisterViewModel(
                        _auth,
                        Session,
                        retour: () => CurrentViewModel = LoginVM,
                        inscriptionReussie: () => CurrentViewModel = HomeVM);

            HomeVM = new HomeViewModel(
                voirJeux: () => CurrentViewModel = GameVM,
                jouer: StartSudoku,
                voirClassement: () => CurrentViewModel = LeaderboardVM);


            GameVM = new GameViewModel( 
                retour: () => CurrentViewModel = HomeVM,
                jouer: StartSudoku);

            LeaderboardVM = new LeaderboardViewModel( retour: () => CurrentViewModel = HomeVM);

            AccueilCommande = new RelaisCommande(() => CurrentViewModel = HomeVM);
            JeuxCommande = new RelaisCommande(() => CurrentViewModel = GameVM);
            ClassementCommande = new RelaisCommande(() => CurrentViewModel = LeaderboardVM);
            ConnexionCommande = new RelaisCommande(() => CurrentViewModel = LoginVM);
            SudokuCommande = new RelaisCommande(StartSudoku);

            DeconnexionCommande = new RelaisCommande(() =>
            {
                Session.Fermer();
                CurrentViewModel = HomeVM;
            });


            _currentViewModel = HomeVM;
        }

        // Lancement du Sudoku après sélection de la difficulté
        private async void StartSudoku()
        {
            if (!Session.EstConnecte)
            {
                CurrentViewModel = LoginVM;
                return;
            }

            // Le Sudoku doit être accessible au département du joueur
            bool accessible;
            try
            {
                accessible = await _sessions.EstAccessibleAsync(Session.Utilisateur!.DepartmentId, SeedData.SudokuGameId);
            }
            catch (Exception)
            {
                MessageBox.Show("Connexion à la base de données impossible. Réessayez plus tard.");
                return;
            }

            if (!accessible)
            {
                MessageBox.Show("Le Sudoku n'est pas disponible pour votre département.",
                                "Accès refusé", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DifficultyWindow window = new DifficultyWindow();
            window.Owner = Application.Current.MainWindow;

            if (window.ShowDialog() == true)
            {
                SudokuVM = new SudokuViewModel(
                    retour: () => CurrentViewModel = HomeVM,
                    difficulty: window.SelectedDifficulty,
                    session: Session,
                    sessions: _sessions);
                OnPropertyChanged(nameof(SudokuVM));
                CurrentViewModel = SudokuVM;
            }
        }





    }
}
