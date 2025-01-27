using Business.Controllers;
using Business.Utilities;
using Entities;
using Presentation.Views;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.ComponentModel;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class ViewPrivateCustomerVM : BaseVM, IDataErrorInfo
    {
        private readonly MainVM _mainViewModel;
        private readonly EmployeeController _employeeController;
        private readonly PrivateCustomerController _privateCustomerController;
        private bool isSaveAttempted = false;
        private bool _isSnackbarActive;


        public ViewPrivateCustomerVM(MainVM mainViewModel, PrivateCustomer privateCustomer)
        {
            _mainViewModel = mainViewModel;
            PrivateCustomer = privateCustomer;
            _privateCustomerController = new PrivateCustomerController();
            _employeeController = new EmployeeController();
            PrivateCustomer.CustomerProspectInformation = _privateCustomerController.GetCustomerProspectInformation(PrivateCustomer.Id);
            ValidationError = string.Empty;
            FirstName = PrivateCustomer.FirstName;
            LastName = PrivateCustomer.LastName;
            StreetAddress = PrivateCustomer.StreetAddress;
            ZipCode = PrivateCustomer.ZipCode;
            City = PrivateCustomer.City;
            Email = PrivateCustomer.Email;
            WorkPhoneNumber = PrivateCustomer.WorkPhoneNumber;
            MobilePhoneNumber = PrivateCustomer.MobilePhoneNumber;
            CheckUser();
            RemoveButtonEnabled();
        }

        private void CheckUser()
        {
            var userPermissions = UserSession.GetInstance().User.Permission;
            SellerFunctions = userPermissions == "Säljare" || userPermissions == "Försäljningsassistent";
        }

        private void RemoveButtonEnabled()
        {
            _removeActive = true;
            if (_privateCustomerController.HasActiveInsurance(PrivateCustomer))
            {
                _removeActive = false;
                return;
            }
        }

        private PrivateCustomer _privateCustomer;
        public PrivateCustomer PrivateCustomer
        {
            get => _privateCustomer;
            set
            {
                if (_privateCustomer != value)
                {
                    _privateCustomer = value;
                    OnPropertyChanged(nameof(PrivateCustomer));
                }
            }
        }

        private string phoneNumber;
        public string WorkPhoneNumber
        {
            get => phoneNumber;
            set
            {
                phoneNumber = value;
                OnPropertyChanged(nameof(WorkPhoneNumber));
            }
        }

        private bool _removeActive;
        public bool removeActive
        {
            get => _removeActive;
            set
            {
                _removeActive = value;
                OnPropertyChanged(nameof(removeActive));
            }
        }

        private string mobilePhoneNumber;
        public string MobilePhoneNumber
        {
            get => mobilePhoneNumber;
            set
            {
                mobilePhoneNumber = value;
                OnPropertyChanged(nameof(MobilePhoneNumber));
            }
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

        public bool SellerFunctions { get; private set; }

        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new RelayCommand(() =>
        {
            _mainViewModel.CurrentView = new HandleCustomerUC
            {
                DataContext = new HandleCustomerVM(_mainViewModel)
            };
        });

        private string firstName;
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        private string lastName;
        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        private string street;
        public string StreetAddress
        {
            get => street;
            set
            {
                street = value;
                OnPropertyChanged(nameof(StreetAddress));
            }
        }

        private string zipCode;
        public string ZipCode
        {
            get => zipCode;
            set
            {
                zipCode = value;
                OnPropertyChanged(nameof(ZipCode));
            }
        }

        private string city;
        public string City
        {
            get => city;
            set
            {
                city = value;
                OnPropertyChanged(nameof(City));
            }
        }

        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
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

        private void UpdatePrivatecustomer()
        {
            isSaveAttempted = true;

            string validationError = string.Empty;
            var validationFields = new List<string>
            {
                nameof(FirstName),
                nameof(LastName),
                nameof(ZipCode),
                nameof(City),
                nameof(StreetAddress),
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

            PrivateCustomer.FirstName = FirstName;
            PrivateCustomer.LastName = LastName;
            PrivateCustomer.StreetAddress = StreetAddress;
            PrivateCustomer.ZipCode = ZipCode;
            PrivateCustomer.City = City;
            PrivateCustomer.Email = Email;
            PrivateCustomer.WorkPhoneNumber = WorkPhoneNumber;
            PrivateCustomer.MobilePhoneNumber = MobilePhoneNumber;

            _privateCustomerController.UpdatePrivateCustomer(PrivateCustomer);
            SnackbarMessage = "Kund har uppdaterats.";
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
                            result = "Ort får inte vara tom.";
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

        private ICommand _removePrivatecustomerCommand;
        public ICommand RemovePrivatecustomerCommand => _removePrivatecustomerCommand ??= new RelayCommand(() =>
        {
            if (_privateCustomerController.HasActiveInsurance(PrivateCustomer))
            {
                return;
            }
            try
            {
                _privateCustomerController.DeletePrivateCustomer(PrivateCustomer);
                SnackbarMessage = "Kund har raderats.";
            }

            catch (Exception ex)
            {
                SnackbarMessage = "Ett fel uppstod vid borttagning av kunden.";
            }

        });


        private ICommand _updatePrivatecustomerCommand;
        public ICommand UpdatePrivatecustomerCommand => _updatePrivatecustomerCommand ??= new RelayCommand(() =>
        {
            UpdatePrivatecustomer();

        });

        private ICommand _removePopUpCommand;
        public ICommand RemovePopUpCommand => _removePopUpCommand ??= new RelayCommand(() =>
        {
            _mainViewModel.IsOverlayVisible = true;
            var removeCustomerPopup = new RemoveCustomerPopup();
            var removeCustomerVM = new RemoveCustomerVM(PrivateCustomer, removeCustomerPopup, _mainViewModel, _privateCustomerController);

            removeCustomerPopup.DataContext = removeCustomerVM;
            removeCustomerPopup.Closed += (s, e) =>
            {
                _mainViewModel.IsOverlayVisible = false;
            };

            removeCustomerPopup.ShowDialog();
        });

        private void OpenProspectInfoPopup()
        {
            var popupWindow = new ProspectInfoPopUp();
            var popupViewModel = new ProspectInformationVM(this, popupWindow);
            popupWindow.DataContext = popupViewModel;
            popupWindow.ShowDialog();
        }

        public void SaveProspectInformation(string outcome)
        {
            var userSession = UserSession.GetInstance();
            var currentUser = userSession.User;
            int agentNumber = _employeeController.GetAgentNumberByEmployeeNumber(currentUser.EmployeeNumber);
            var employee = _employeeController.GetEmployeeByAgentNumber(agentNumber);
            var prospectInfo = new CustomerProspectInformation
            {
                AgentNumber = agentNumber,
                ContactDate = DateTime.Now,
                Outcome = outcome,
                CustomerId = PrivateCustomer.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName
            };
            _privateCustomerController.AddProspectInformation(prospectInfo);
            PrivateCustomer.CustomerProspectInformation.Add(prospectInfo);
        }

        private ICommand _handleProspectCommand;
        public ICommand HandleProspectCommand => _handleProspectCommand ??= new RelayCommand(() =>
        {
            if (!SellerFunctions) return;
            OpenProspectInfoPopup();
        });
    }
}
