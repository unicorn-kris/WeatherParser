using System;
using System.Windows.Input;

namespace WeatherParser.WPF
{
    public class RelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action<object?> _action;

        private readonly Func<object?, bool>? _funcAction;

        public RelayCommand(Action<object?> action, Func<object?, bool>? funcAction = null)
        {
            _action = action;
            _funcAction = funcAction;
        }

        public virtual bool CanExecute(object? parameter) => _funcAction?.Invoke(parameter) ?? true;

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
