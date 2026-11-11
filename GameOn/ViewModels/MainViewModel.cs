using GameOn.Commande;
using GameOn.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.ViewModels
{
    public class MainViewModel : BaseViewModel
    {

        private readonly AuthService _auth = new AuthService();
        private BaseViewModel _currentViewModel;

        public LoginViewModel LoginVM { get; }
        public RegisterViewModel RegisterVM { get; }
        public HomeViewModel HomeVM { get; }
        public GameViewModel GameVM { get; }
        public LeaderboardViewModel LeaderboardVM { get; }
        public SudokuViewModel SudokuVM { get; }

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
                jouer: () => CurrentViewModel = SudokuVM,
                voirClassement: () => CurrentViewModel = LeaderboardVM);


            GameVM = new GameViewModel( 
                retour: () => CurrentViewModel = HomeVM,
                jouer: () => CurrentViewModel = SudokuVM);

            LeaderboardVM = new LeaderboardViewModel( retour: () => CurrentViewModel = HomeVM);

            SudokuVM = new SudokuViewModel(retour: () => CurrentViewModel = HomeVM);

            AccueilCommande = new RelaisCommande(() => CurrentViewModel = HomeVM);
            JeuxCommande = new RelaisCommande(() => CurrentViewModel = GameVM);
            ClassementCommande = new RelaisCommande(() => CurrentViewModel = LeaderboardVM);
            ConnexionCommande = new RelaisCommande(() => CurrentViewModel = LoginVM);
            SudokuCommande = new RelaisCommande(() => CurrentViewModel = SudokuVM);

            DeconnexionCommande = new RelaisCommande(() =>
            {
                Session.Fermer();
                CurrentViewModel = HomeVM;
            });


            _currentViewModel = HomeVM;
        }




    }
}
