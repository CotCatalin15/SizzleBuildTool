using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BuildTool
{
    internal class RelayCommand : ICommand
    {

        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public  event EventHandler? CanExecuteChanged 
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public RelayCommand(Action<object>? Execute, Func<object, bool>? CanExecute = null)
        {
            _execute = Execute;
            _canExecute = CanExecute; 
        }


        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
