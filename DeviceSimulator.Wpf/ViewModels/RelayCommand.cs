using System;
using System.Windows.Input;

namespace DeviceSimulator.Wpf.ViewModels
{
    public class RelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (CanExecuteFunc == null)
                return true;
            return CanExecuteFunc(parameter);
        }

        public void Execute(object? parameter)
        {
            if (ExecuteAction == null)
                return;
            ExecuteAction(parameter);
        }

        public Func<object?, bool>? CanExecuteFunc { get; set; }
        public Action<object?>? ExecuteAction { get; set; }
    }
}