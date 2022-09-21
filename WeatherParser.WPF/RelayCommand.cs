using System;
using System.Windows.Input;

namespace WeatherParser.WPF
{
    public class RelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action<object?> _action;

        public RelayCommand(Action<object?> action)
        {
            _action = action;
        }

        public virtual bool CanExecute(object? parameter) => true;

        public virtual void Execute(object? parameter)
        {
            _action?.Invoke(parameter);
        }

        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
