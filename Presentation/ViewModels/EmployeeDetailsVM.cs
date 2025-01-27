using Business.Controllers;
using Entities;
using Presentation.Views;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Presentation.ViewModels
{
    public class EmployeeDetailsVM : BaseVM, IDataErrorInfo
    {
        private readonly DispatcherTimer _snackbarTimer;
        private MainVM _mainViewModel;
        private Employee _employee;
        private ObservableCollection<User> _users;
        private ObservableCollection<Employee> _employees;
        private ObservableCollection<EmployeeRole> _employeeRoles;
        private EmployeeController _employeeController;
        private UserController _userController;
        private bool isSaveAttempted = false;
        private bool _isSnackbarActive;

        private int _employmentPercentage;
        public EmployeeDetailsVM(MainVM mainViewModel, Employee employee)
        {
            _mainViewModel = mainViewModel;
            _employee = employee;
            _employeeController = new EmployeeController();
            _userController = new UserController();
            ValidationError = string.Empty;
            _snackbarTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _snackbarTimer.Tick += HideSnackbar;
            Email = _employee.Email;
            PhoneNumber = _employee.PhoneNumber;
            StreetAddress = _employee.StreetAddress;
            ZipCode = _employee.ZipCode;
            City = _employee.City;
            LoadEmployeeDetails();
        }

        public bool IsSnackbarActive
        {
            get => _isSnackbarActive;
            set
            {
                _isSnackbarActive = value;
                OnPropertyChanged(nameof(IsSnackbarActive));

                if (value) // Om snackbar aktiveras, starta timern
                {
                    _snackbarTimer.Start();
                }
            }
        }

        private void HideSnackbar(object sender, EventArgs e)
        {
            // Stoppa timern och döljer snackbar-meddelandet
            _snackbarTimer.Stop();
            IsSnackbarActive = false;
        }

        private string _snackbarMessage;
        public string SnackbarMessage
        {
            get => _snackbarMessage;
            set
            {
                _snackbarMessage = value;
                OnPropertyChanged(nameof(SnackbarMessage));
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
        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        private string _email;
        private string _phoneNumber;
        private string _streetAddress;
        private string _zipCode;
        private string _city;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }

        public string StreetAddress
        {
            get => _streetAddress;
            set
            {
                _streetAddress = value;
                OnPropertyChanged(nameof(StreetAddress));
            }
        }

        public string ZipCode
        {
            get => _zipCode;
            set
            {
                _zipCode = value;
                OnPropertyChanged(nameof(ZipCode));
            }
        }

        public string City
        {
            get => _city;
            set
            {
                _city = value;
                OnPropertyChanged(nameof(City));
            }
        }

        private void NotifyValidation(string propertyName)
        {
            OnPropertyChanged(propertyName);
            ValidationError = this[propertyName];
        }

        public Employee Employee
        {
            get => _employee;
            set
            {
                _employee = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<User> Users
        {
            get => _users ??= new ObservableCollection<User>();
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }
        public int EmploymentPercentage
        {
            get => _employmentPercentage;
            set
            {
                _employmentPercentage = value;
                OnPropertyChanged(nameof(EmploymentPercentage));

            }
        }

        private readonly List<int> _employmentPercentages = new List<int> { 25, 50, 75, 100 };
        public List<int> EmploymentPercentages => _employmentPercentages;

        public ObservableCollection<Employee> Employees
        {
            get => _employees ??= new ObservableCollection<Employee>();
            set
            {
                _employees = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<EmployeeRole> EmployeeRoles
        {
            get => _employeeRoles ??= new ObservableCollection<EmployeeRole>();
            set
            {
                _employeeRoles = value;
                OnPropertyChanged();
            }
        }

        // Laddar både användare och roller för den anställda
        private void LoadEmployeeDetails()
        {
            LoadUsers();
            LoadEmployeeRoles();
        }

        // Metod för att hämta användare kopplade till den anställda
        private void LoadUsers()
        {
            var usersList = _employeeController.GetUsersByEmployeeNumber(Employee.EmployeeNumber);
            Users = new ObservableCollection<User>(usersList);
        }

        // Metod för att hämta roller kopplade till den anställda
        private void LoadEmployeeRoles()
        {
            var rolesList = _employeeController.GetEmployeeRolesByEmployeeNumber(Employee.EmployeeNumber);
            EmployeeRoles = new ObservableCollection<EmployeeRole>(rolesList);
        }

        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new RelayCommand(GoBack);

        private void GoBack()
        {
            _mainViewModel.CurrentView = new HandleEmployeeUC
            {
                DataContext = new HandleEmployeeVM(_mainViewModel)
            };
        }
        private ICommand _removeUserCommand;
        public ICommand RemoveUserCommand => _removeUserCommand ??= new RelayCommand<User>(RemoveUser);

        private void RemoveUser(User user)
        {
            if (user != null)
            {

                MessageBoxResult result = MessageBox.Show(
                "Är du säker på att du vill ta bort användaren",
                "Bekräfta borttagning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    // Ta bort anställd
                    _userController.RemoveUser(user);
                    Users.Remove(user);
                }
            }
        }
        private ICommand _removePopUpCommand;
        public ICommand RemovePopUpCommand => _removePopUpCommand ??= new RelayCommand(() =>
        {
            _mainViewModel.IsOverlayVisible = true; // Visa överlagring
            var removeEmployeePopup = new RemoveEmployeePopUp();
            var removeEmployeeVM = new RemoveEmployeeVM(Employee, removeEmployeePopup, _mainViewModel, this, _employeeController);
            removeEmployeePopup.DataContext = removeEmployeeVM;
            removeEmployeePopup.Closed += (s, e) =>
            {
                _mainViewModel.IsOverlayVisible = false; // Dölja överlagring när popup-fönstret stängs
            };

            removeEmployeePopup.ShowDialog();
        });

        private ICommand _updateEmployeeCommand;
        public ICommand UpdateEmployeeCommand => _updateEmployeeCommand ??= new RelayCommand(UpdateEmployee);

        private void UpdateEmployee()
        {
            isSaveAttempted = true;
            string validationError = string.Empty;
            var validationFields = new List<string>
            {
                nameof(ZipCode),
                nameof(City),
                nameof(StreetAddress),
                nameof(PhoneNumber),
                nameof(Email)
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
                return;
            }

            Employee.ZipCode = ZipCode;
            Employee.City = City;
            Employee.StreetAddress = StreetAddress;
            Employee.PhoneNumber = PhoneNumber;
            Employee.Email = Email;
            _employeeController.UpdateEmployee(Employee);

            SnackbarMessage = "Anställd uppdaterad";
            IsSnackbarActive = true;
        }

        public string this[string columnName]
        {
            get
            {
                if (!isSaveAttempted) return string.Empty;

                string result = string.Empty;

                switch (columnName)
                {
                    case nameof(StreetAddress):
                        if (string.IsNullOrEmpty(StreetAddress))
                        {
                            result = "Gatuadress får inte vara tom.";
                        }
                        break;

                    case nameof(ZipCode):
                        if (string.IsNullOrEmpty(ZipCode))
                        {
                            result = "Postnummer får inte vara tomt.";
                        }
                        break;

                    case nameof(City):
                        if (string.IsNullOrEmpty(City))
                        {
                            result = "Stad får inte vara tom.";
                        }
                        break;

                    case nameof(PhoneNumber):
                        if (string.IsNullOrEmpty(PhoneNumber))
                        {
                            result = "Mobilnummer får inte vara tomt.";
                        }
                        break;

                    case nameof(Email):
                        if (string.IsNullOrEmpty(Email))
                        {
                            result = "Mejl får inte vara tom.";
                        }
                        else if (!Email.Contains("@"))
                        {
                            result = "Mejl måste innehålla '@'.";
                        }
                        break;
                }
                return result;
            }
        }

        private ICommand _addUserViewCommand;
        public ICommand AddUserViewCommand => _addUserViewCommand ??= new RelayCommand(() =>
        {
            var addUserVM = new AddUserVM(_mainViewModel);
            var addUserUC = new AddUserUC { DataContext = addUserVM };
            _mainViewModel.CurrentView = addUserUC;
        });

        private ICommand _updatePasswordCommand;
        public ICommand UpdatePasswordCommand => _updatePasswordCommand ??= new RelayCommand<User>(UpdatePassword);
        private void UpdatePassword(User user)
        {
            if (user != null)
            {

                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    SnackbarMessage = "Lösenordsfältet kan inte vara tomt!";
                    IsSnackbarActive = true;
                    return;
                }
                var currentUser = _userController.GetUserById(user.Id);
                // Kontrollera om det nya lösenordet är samma som det gamla
                if (currentUser.Password == user.Password)
                {
                    SnackbarMessage = "Det nya lösenordet kan inte vara samma som det gamla!";
                    IsSnackbarActive = true;
                    return;
                }

                currentUser.Password = user.Password;
                _userController.UpdateUser(currentUser);
                IsSnackbarActive = true;
                SnackbarMessage = "Lösenordet har uppdaterats framgångsrikt!";
            }
            else
            {
                SnackbarMessage = "Ingen användare vald!";
                IsSnackbarActive = true;
            }
        }

        private ICommand _addEmployeeRoleCommand;
        public ICommand AddEmployeeRoleCommand => _addEmployeeRoleCommand ??= new RelayCommand(() =>
        {
            var addEmployeeRolePopup = new AddEmployeeRolePopup();
            var addEmployeeRoleVM = new AddEmployeeRoleVM(addEmployeeRolePopup, _mainViewModel, Employee);
            addEmployeeRolePopup.DataContext = addEmployeeRoleVM;
            addEmployeeRolePopup.ShowDialog();
        });
    }
}
