using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.ViewModels
{
    public class MainViewModel : BaseViewModel
    {

        private BaseViewModel _currentViewModel;

        public LoginViewModel LoginVM { get; }
        public RegisterViewModel RegisterVM { get; }
        public HomeViewModel HomeVM { get; }
        public GameViewModel GameVM { get; }
        public LeaderboardViewModel LeaderboardVM { get; }

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
                allerInscription: () => CurrentViewModel = RegisterVM,
                seConnecter: () => CurrentViewModel = HomeVM);

            RegisterVM = new RegisterViewModel(retour: () => CurrentViewModel = LoginVM);

            HomeVM = new HomeViewModel(
                                        jouer: () => CurrentViewModel = GameVM,
                                        classement: () => CurrentViewModel = LeaderboardVM,
                                        deconnexion: () => CurrentViewModel = LoginVM);


            GameVM = new GameViewModel( retour: () => CurrentViewModel = HomeVM);

            LeaderboardVM = new LeaderboardViewModel( retour: () => CurrentViewModel = HomeVM);

            CurrentViewModel = LoginVM;
        }




    }
}
