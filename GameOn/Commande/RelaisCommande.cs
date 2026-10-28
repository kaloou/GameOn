using System.Windows.Input;

namespace GameOn.Commande
{
    public class RelaisCommande : ICommand
    {
        // Délégué pour l'exécution de la commande
        private readonly Action _commande;

        // Délégué pour vérifier si la commande peut être exécutée
        private readonly Func<bool>? _executable;

        public RelaisCommande(Action commande, Func<bool>? executable = null)
        {
            _commande = commande ?? throw new ArgumentNullException(nameof(commande));
            _executable = executable;
        }

        #region Implémentation de l'interface ICommand

        // WPF réévalue CanExecute automatiquement (clic, frappe, focus...)
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        // Méthode pour vérifier si la commande peut être exécutée
        public bool CanExecute(object? parameter) => _executable == null || _executable();

        // Méthode pour exécuter la commande
        public void Execute(object? parameter) => _commande();

        #endregion
    }
}