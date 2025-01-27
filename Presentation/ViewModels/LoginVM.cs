using Business.Controllers;
using Entities;
using Presentation.Views;
using PresentationLayer.Commands;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class LoginVM : BaseVM, IDataErrorInfo
    {
        private string _username;
        private string _password;
        private bool _isLoading;
        private readonly LoginController _loginController;
        private bool isSaveAttempted = false;
        public User LoggedInUser { get; private set; }

        public LoginVM()
        {
            _loginController = new LoginController();
            ValidationError = string.Empty;
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private string _validationError;
        public string ValidationError
        {
            get => _validationError;
            set
            {
                _validationError = value;
                OnPropertyChanged(nameof(ValidationError));
            }
        }

        public string Error => string.Empty;

        private void NotifyValidation(string propertyName)
        {
            OnPropertyChanged(propertyName);
            ValidationError = this[propertyName];
        }

        private ICommand _logInCommand;
        public ICommand LogInCommand => _logInCommand ??= new RelayCommand(async () => await LogInAsync());

        private async Task LogInAsync()
        {
            IsLoading = true;
            await Task.Delay(1500);

            LoggedInUser = _loginController.Authenticate(Username, Password);

            if (LoggedInUser != null)
            {
                MainView newMainView = new MainView();
                newMainView.Show();
                IsLoading = false;
                Application.Current.MainWindow.Close();
            }
            else
            {
                IsLoading = false;
                isSaveAttempted = true;
                string validationError = string.Empty;
                var validationFields = new List<string> { nameof(Username), nameof(Password) };

                foreach (var field in validationFields)
                {
                    string fieldError = this[field];
                    if (!string.IsNullOrEmpty(fieldError))
                    {
                        validationError += fieldError + "\n";
                        NotifyValidation(field);
                    }
                }

                if (!string.IsNullOrEmpty(validationError))
                {
                    ValidationError = validationError;
                }
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (!isSaveAttempted) return string.Empty;
                string result = string.Empty;

                switch (columnName)
                {
                    case nameof(Username):
                    case nameof(Password):
                        if (LoggedInUser == null)
                        {
                            result = "Användarnamn eller lösenord är fel";
                        }
                        break;
                }
                return result;
            }
        }
    }
}
