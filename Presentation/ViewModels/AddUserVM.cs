using Business.Controllers;
using Entities;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class AddUserVM : BaseVM, IDataErrorInfo
    {
        private readonly UserController _userController;
        private readonly MainVM _mainViewModel;
        private readonly EmployeeController _employeeController;
        private bool isSaveAttempted = false;
        private bool _isSnackbarActive;

        public AddUserVM(MainVM mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _userController = new UserController();
            EmploymentNumbers = new ObservableCollection<int>();
            Permissions = new ObservableCollection<string>();
            _employeeController = new EmployeeController();
            LoadData();
        }

        public bool IsSnackbarActive
        {
            get => _isSnackbarActive;
            set
            {
                _isSnackbarActive = value;
                OnPropertyChanged(nameof(IsSnackbarActive));
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

        private ObservableCollection<int> _employmentNumbers;
        public ObservableCollection<int> EmploymentNumbers
        {
            get => _employmentNumbers;
            set
            {
                _employmentNumbers = value;
                OnPropertyChanged(nameof(EmploymentNumbers));
            }
        }

        private ObservableCollection<string> _permissions;
        public ObservableCollection<string> Permissions
        {
            get => _permissions;
            set
            {
                _permissions = value;
                OnPropertyChanged(nameof(Permissions));
            }
        }

        private int _selectedEmploymentNumber;
        public int SelectedEmploymentNumber
        {
            get => _selectedEmploymentNumber;
            set
            {
                if (_selectedEmploymentNumber != value)
                {
                    _selectedEmploymentNumber = value;
                    OnPropertyChanged(nameof(SelectedEmploymentNumber));
                    if (_selectedEmploymentNumber != 0)
                    {
                        LoadData();
                    }
                }
            }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _selectedPermission;
        public string SelectedPermission
        {
            get => _selectedPermission;
            set
            {
                _selectedPermission = value;
                OnPropertyChanged(nameof(SelectedPermission));
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        private string _passwordTwo;
        public string PasswordTwo
        {
            get => _passwordTwo;
            set
            {
                _passwordTwo = value;
                OnPropertyChanged(nameof(PasswordTwo));
            }
        }

        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new RelayCommand(() =>
        {
            _mainViewModel.CurrentView = new HandleEmployeeUC
            {
                DataContext = new HandleEmployeeVM(_mainViewModel)
            };
        });

        private ICommand _saveUserCommand;
        public ICommand SaveUserCommand => _saveUserCommand ??= new RelayCommand(() =>
        {
            isSaveAttempted = true;

            string validationError = string.Empty;
            var validationFields = new List<string>
            {
                nameof(Username),
                nameof(Password),
                nameof(PasswordTwo),
                nameof(SelectedPermission),
                nameof(SelectedEmploymentNumber)
            };

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
                IsSnackbarActive = true;
                return;
            }

            try
            {
                User user = new User
                {
                    Username = Username,
                    Password = Password,
                    EmployeeNumber = SelectedEmploymentNumber,
                    Permission = SelectedPermission,
                };

                _userController.AddUser(user);
                ValidationError = "Användare tillagd!";
                IsSnackbarActive = true;
            }
            catch
            {
                ValidationError = "Ett fel uppstod vid tillägg av användaren.";
                IsSnackbarActive = true;
            }
        });

        public string this[string columnName]
        {
            get
            {
                if (!isSaveAttempted) return string.Empty;

                string result = string.Empty;

                switch (columnName)
                {
                    case nameof(Username):
                        if (string.IsNullOrWhiteSpace(Username))
                        {
                            result = "Användarnamn får inte vara tomt.";
                        }
                        else if (_userController.GetUsers().Any(u => u.Username == Username))
                        {
                            result = "Användarnamnet existerar redan.";
                        }
                        break;

                    case nameof(Password):
                        if (string.IsNullOrWhiteSpace(Password))
                        {
                            result = "Lösenord får inte vara tomt.";
                        }
                        else if (Password.Length < 8)
                        {
                            result = "Lösenordet måste vara minst 8 tecken långt.";
                        }
                        break;

                    case nameof(PasswordTwo):
                        if (string.IsNullOrEmpty(PasswordTwo))
                        {
                            result = "Bekräfta lösenord får inte vara tomt.";
                        }
                        else if (Password != PasswordTwo)
                        {
                            result = "Lösenorden matchar inte.";
                        }
                        break;

                    case nameof(SelectedPermission):
                        if (string.IsNullOrEmpty(SelectedPermission))
                        {
                            result = "Vänligen välj en giltig behörighet.";
                        }
                        break;

                    case nameof(SelectedEmploymentNumber):
                        if (SelectedEmploymentNumber == 0)
                        {
                            result = "Vänligen välj ett giltigt anställningsnummer.";
                        }
                        break;
                }

                return result;
            }
        }

        public string Error => string.Empty;

        private void NotifyValidation(string propertyName)
        {
            OnPropertyChanged(propertyName);
            ValidationError = this[propertyName];
        }

        private void LoadData()
        {
            if (EmploymentNumbers.Count == 0)
            {
                _userController.GetEmploymentNumbers().ForEach(EmploymentNumbers.Add);
            }

            if (SelectedEmploymentNumber != 0)
            {
                var employeeRoles = _employeeController.GetEmployeeRolesByEmployeeNumber(SelectedEmploymentNumber);
                Permissions.Clear();
                foreach (var role in employeeRoles)
                {
                    Permissions.Add(role.Role.Name);
                }
            }
            else
            {
                Permissions = new ObservableCollection<string> { "Admin", "Ekonomiassistent", "Säljare", "Försäljningschef", "VD", "Försäljningsassistent" };
            }
        }
    }
}
