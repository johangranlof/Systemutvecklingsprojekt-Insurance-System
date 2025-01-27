using System.Windows.Input;

namespace PresentationLayer.Commands
{
    public abstract class CommandBase : ICommand // En abstrakt klass som implementerar ICommand-gränssnittet och används för att hantera kommandon
    {
        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
