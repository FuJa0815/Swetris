using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Swetris
{
    internal class Command : ICommand
    {
        private readonly Action<object> _action;
        private readonly Predicate<object> _canExecute;
        public Command(Action<object> action, Predicate<object> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute ?? (p=>true);
        }

        public bool CanExecute(object parameter) => _canExecute(parameter);

        public void Execute(object parameter) => _action(parameter);

        public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());

        public event EventHandler CanExecuteChanged;
    }
}
