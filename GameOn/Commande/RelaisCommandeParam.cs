using System.Windows.Input;

namespace GameOn.Commande
{
    public class RelaisCommandeParam : ICommand
    {
        // Délégué pour l'exécution de la commande, avec le paramètre venant du XAML (CommandParameter)
        private readonly Action<object?> _commande;

        // Délégué pour vérifier si la commande peut être exécutée, selon le paramètre
        private readonly Func<object?, bool>? _executable;

        public RelaisCommandeParam(Action<object?> commande, Func<object?, bool>? executable = null)
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
        public bool CanExecute(object? parameter) => _executable == null || _executable(parameter);

        // Méthode pour exécuter la commande
        public void Execute(object? parameter) => _commande(parameter);

        #endregion
    }
}