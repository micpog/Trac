using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace TrackerApp
{
    public class RelayCommand : ICommand
    {
        private readonly Action _targetExecuteMethod;
        private readonly Func<bool> _canExecuteMethod;

        public RelayCommand(Action targetExecuteMethod, Func<bool> canExecuteMethod)
        {
            _canExecuteMethod = canExecuteMethod;
            _targetExecuteMethod = targetExecuteMethod;
        }

        public RelayCommand(Action targetExecuteMethod)
        {
            _targetExecuteMethod = targetExecuteMethod;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecuteMethod != null)
            {
                return _canExecuteMethod();
            }

            return _targetExecuteMethod != null;
        }

        public void Execute(object parameter)
        {
            _targetExecuteMethod?.Invoke();
        }

        public event EventHandler CanExecuteChanged;
    }
}
