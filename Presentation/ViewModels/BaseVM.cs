using Presentation.Helpers;
using Presentation.Views;
using PresentationLayer.Commands;
using System.Windows;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class BaseVM : ObservableObject
    {
        public BaseVM() { }

        protected ICommand _logoutCommand = null!;
        public ICommand LogoutCommand => _logoutCommand ??= new RelayCommand(Logout);

        private void Logout()
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.DataContext == this);

            if (currentWindow != null)
            {
                LoginView loginView = new LoginView();
                Application.Current.MainWindow = loginView;
                loginView.Show();
                currentWindow.Close();
            }
        }
    }
}
