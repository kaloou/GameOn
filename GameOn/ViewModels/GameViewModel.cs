using GameOn.Commande;
using System.Windows.Threading;

namespace GameOn.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        public RelaisCommande RetourCommande { get; }
        public RelaisCommande JouerCommande { get; }

        public string TempsRestant => (DateTime.Today.AddDays(1) - DateTime.Now).ToString(@"hh\:mm\:ss");
        public double ProgressionJour => DateTime.Now.TimeOfDay.TotalDays;

        public GameViewModel(Action retour, Action jouer)
        {
            RetourCommande = new RelaisCommande(retour);
            JouerCommande = new RelaisCommande(jouer);

            var minuteur = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            minuteur.Tick += (_, _) => OnAllPropertiesChanged();
            minuteur.Start();
        }
    }
}
