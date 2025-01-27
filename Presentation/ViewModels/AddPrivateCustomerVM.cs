using Business.Controllers;
using Entities;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.ComponentModel;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class AddPrivateCustomerVM : BaseVM, IDataErrorInfo
    {
        private readonly PrivateCustomerController _privateCustomerController;
        private readonly MainVM _mainViewModel;
        private bool isSaveAttempted = false;
        private bool _isSnackbarActive;

        public AddPrivateCustomerVM(MainVM mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _privateCustomerController = new PrivateCustomerController();
            ValidationError = string.Empty;
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

        private string ssn;
        public string Ssn
        {
            get => ssn;
            set
            {
                string digitsOnly = value.Replace("-", "").Replace(" ", "");

                if (digitsOnly.Length > 10)
                    return;

                ssn = digitsOnly;
                OnPropertyChanged(nameof(Ssn));
            }
        }

        private string street;
        public string Street
        {
            get => street;
            set
            {
                street = value;
                OnPropertyChanged(nameof(Street));
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

        private string phoneNumber;
        public string PhoneNumber
        {
            get => phoneNumber;
            set
            {
                phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
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

                    case nameof(Ssn):
                        if (string.IsNullOrEmpty(Ssn))
                        {
                            result = "Personnummer får inte vara tomt.";
                        }
                        else if (Ssn.Length != 10 || !System.Text.RegularExpressions.Regex.IsMatch(Ssn, @"^\d{10}$"))
                        {
                            result = "Personnummer måste innehålla 10 siffror.";
                        }
                        else if (_privateCustomerController.IsSsnExists(Ssn))
                        {
                            result = "Personnummer finns redan registrerat";
                        }
                        break;

                    case nameof(Street):
                        if (string.IsNullOrEmpty(Street))
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

                    case nameof(Email):
                        if (!string.IsNullOrEmpty(Email) && !Email.Contains("@"))
                        {
                            result = "Mejl måste innehålla '@'.";
                        }
                        break;
                }

                return result;
            }
        }

        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new RelayCommand(() =>
        {
            _mainViewModel.CurrentView = new HandleCustomerUC
            {
                DataContext = new HandleCustomerVM(_mainViewModel)
            };
        });

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(() =>
        {
            isSaveAttempted = true;

            string validationError = string.Empty;
            var validationFields = new List<string>
            {
                nameof(FirstName),
                nameof(LastName),
                nameof(Ssn),
                nameof(ZipCode),
                nameof(City),
                nameof(Street),
                nameof(MobilePhoneNumber),
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

            PrivateCustomer privateCustomer = new PrivateCustomer
            {
                FirstName = FirstName,
                LastName = LastName,
                SSN = Ssn,
                StreetAddress = Street,
                ZipCode = ZipCode,
                City = City,
                IsPrivateCustomer = true,
                MobilePhoneNumber = MobilePhoneNumber,
                WorkPhoneNumber = string.IsNullOrEmpty(PhoneNumber) ? string.Empty : PhoneNumber,
                Email = Email,
            };

            _privateCustomerController.AddPrivateCustomer(privateCustomer);
            ValidationError = "Kund tillagd!";
            IsSnackbarActive = true; // Visa ett framgångsmeddelande via Snackbar

        });

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
    }
}
