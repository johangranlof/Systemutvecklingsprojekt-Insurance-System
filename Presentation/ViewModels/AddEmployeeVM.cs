using Business.Controllers;
using Entities;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.ComponentModel;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class AddEmployeeVM : BaseVM, IDataErrorInfo
    {
        private bool isSaveAttempted = false;
        private bool _isSnackbarActive;

        public AddEmployeeVM(MainVM mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _employeeController = new EmployeeController();
            Roles = _employeeController.GetRoles();
            ValidationError = string.Empty;
        }

        private readonly EmployeeController _employeeController;
        private readonly MainVM _mainViewModel;

        private string _firstName;
        private string _lastName;
        private string _ssn;
        private string _email;
        private string _phoneNumber;
        private string _streetAddress;
        private string _zipCode;
        private string _city;
        private int _selectedRole;
        private int _employmentPercentage;

        private List<Role> _roles;
        public List<Role> Roles
        {
            get => _roles;
            set
            {
                _roles = value;
                OnPropertyChanged(nameof(Roles));
            }
        }

        private int _selectedRoleId;
        public int SelectedRoleId
        {
            get => _selectedRoleId;
            set
            {
                _selectedRoleId = value;
                OnPropertyChanged(nameof(SelectedRoleId));
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string SSN
        {
            get => _ssn;
            set
            {
                _ssn = value;
                OnPropertyChanged(nameof(SSN));
            }
        }


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

        public int SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged(nameof(SelectedRole));
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

        // Går tillbaka till HandleEmployee vyn
        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new RelayCommand(() =>
        {
            _mainViewModel.CurrentView = new HandleEmployeeUC
            {
                DataContext = new HandleEmployeeVM(_mainViewModel)
            };
        });

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

        private int _agentNumber;
        public int AgentNumber
        {
            get => _agentNumber;
            set
            {
                _agentNumber = value;
                OnPropertyChanged(nameof(AgentNumber));
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
                    case nameof(FirstName):
                        if (string.IsNullOrEmpty(FirstName))
                        {
                            result = "Förnamn får inte vara tomt.";
                        }
                        else if (FirstName.Length > 50)
                        {
                            result = "Förnamn får max vara 50 tecken.";
                        }
                        break;

                    case nameof(EmploymentPercentage):


                        if (EmploymentPercentage < 25)
                        {
                            result = "Anställningsgraden måste vara minst 25%";
                        }
                        break;

                    case nameof(LastName):
                        if (string.IsNullOrEmpty(LastName))
                        {
                            result = "Efternamn får inte vara tomt.";
                        }
                        else if (LastName.Length > 50)
                        {
                            result = "Efternamn får max vara 50 tecken.";
                        }
                        break;

                    case nameof(SSN):
                        if (string.IsNullOrEmpty(SSN))
                        {
                            result = "Personnummer får inte vara tomt.";
                        }
                        else if (SSN.Length != 10 || !System.Text.RegularExpressions.Regex.IsMatch(SSN, @"^\d{10}$"))
                        {
                            result = "Personnummer måste innehålla 10 siffror.";
                        }
                        else if (_employeeController.IsSsnExists(SSN))
                        {
                            result = "Personnummer finns redan registrerat";
                        }
                        break;

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
                    case nameof(SelectedRoleId):
                        if (SelectedRoleId <= 0)
                        {
                            result = "Du måste välja en giltig roll.";
                        }
                        break;
                }
                return result;
            }
        }

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }


        private void NotifyValidation(string propertyName)
        {
            OnPropertyChanged(propertyName);
            ValidationError = this[propertyName];
        }

        private ICommand _addEmployeeCommand;
        public ICommand AddEmployeeCommand => _addEmployeeCommand ??= new RelayCommand(() =>
        {

            isSaveAttempted = true;
            string validationError = string.Empty;
            var validationFields = new List<string>
            {
                nameof(FirstName),
                nameof(LastName),
                nameof(SSN),
                nameof(ZipCode),
                nameof(City),
                nameof(EmploymentPercentage),
                nameof(StreetAddress),
                nameof(PhoneNumber),
                nameof(Email),
                nameof(SelectedRoleId)
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

            if (SelectedRoleId == 1)
            {
                AgentNumber = _employeeController.GenerateUniqueAgentNumber();
            }

            EmployeeRole employeeRole = new EmployeeRole
            {
                RoleId = SelectedRoleId,
                EmploymentRate = (int)EmploymentPercentage,
                AgentNumber = AgentNumber
            };

            Employee employee = new Employee
            {
                FirstName = FirstName,
                LastName = LastName,
                SSN = SSN,
                StreetAddress = StreetAddress,
                ZipCode = ZipCode,
                City = City,
                Email = Email,
                PhoneNumber = PhoneNumber,
                EmployeeRoles = new List<EmployeeRole>()
            };

            employee.EmployeeRoles.Add(employeeRole);
            _employeeController.AddEmployee(employee);

            ValidationError = "Anställd tillagd!";
            IsSnackbarActive = true;
        });

        public bool IsSnackbarActive
        {
            get => _isSnackbarActive;
            set
            {
                _isSnackbarActive = value;
                OnPropertyChanged(nameof(IsSnackbarActive));
            }
        }
    }
}
