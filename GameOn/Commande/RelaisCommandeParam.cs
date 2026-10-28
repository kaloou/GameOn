using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace GameOn.Commande
{
    public class RelaisCommandeParam : ICommand
    {
        private readonly Action<object?> _commande;
        private readonly Func<bool>? _executable;

        public RelaisCommandeParam(Action<object?> commande, Func<bool>? executable = null)
        {
            _commande = commande;
            _executable = executable;
        }

        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => _executable == null || _executable();
        public void Execute(object? parameter) => _commande(parameter);
    }
}
