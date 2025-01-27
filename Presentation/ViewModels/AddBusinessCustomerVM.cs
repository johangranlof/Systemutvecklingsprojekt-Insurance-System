using Business.Controllers;
using Entities;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.ComponentModel;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class AddBusinessCustomerVM : BaseVM, IDataErrorInfo
    {
        private readonly BusinessCustomerController _businessCustomerController;
        private readonly MainVM _mainViewModel;
        private bool isSaveAttempted = false;
        private bool _isSnackbarActive;

        public AddBusinessCustomerVM(MainVM mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _businessCustomerController = new BusinessCustomerController();
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

        private string companyName;
        private readonly string organisationNumber;
        private string streetAddress;
        private string city;
        private string zipCode;
        private string areaCode;
        private string phoneNumber;
        private string mobileNumber;
        private string email;

        public string CompanyName
        {
            get => companyName;
            set { companyName = value; OnPropertyChanged(nameof(CompanyName)); }
        }

        private string _organisationNumber;
        public string OrganisationNumber
        {
            get => _organisationNumber;
            set
            {
                string digitsOnly = value.Replace("-", "");

                if (digitsOnly.Length > 10)
                    return;

                if (digitsOnly.Length > 6)
                {
                    _organisationNumber = digitsOnly.Insert(6, "-");
                }
                else
                {
                    _organisationNumber = digitsOnly;
                }

                OnPropertyChanged(nameof(OrganisationNumber));
            }
        }

        public string StreetAddress
        {
            get => streetAddress;
            set { streetAddress = value; OnPropertyChanged(nameof(StreetAddress)); }
        }

        public string City
        {
            get => city;
            set { city = value; OnPropertyChanged(nameof(City)); }
        }

        public string ZipCode
        {
            get => zipCode;
            set { zipCode = value; OnPropertyChanged(nameof(ZipCode)); }
        }

        public string AreaCode
        {
            get => areaCode;
            set { areaCode = value; OnPropertyChanged(nameof(AreaCode)); }
        }

        public string PhoneNumber
        {
            get => phoneNumber;
            set { phoneNumber = value; OnPropertyChanged(nameof(PhoneNumber)); }
        }

        public string MobileNumber
        {
            get => mobileNumber;
            set { mobileNumber = value; OnPropertyChanged(nameof(MobileNumber)); }
        }

        public string Email
        {
            get => email;
            set { email = value; OnPropertyChanged(nameof(Email)); }
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
                    case nameof(CompanyName):
                        if (string.IsNullOrEmpty(CompanyName))
                        {
                            result = "Företagsnamn får inte vara tomt.";
                        }
                        else if (CompanyName.Length > 50)
                        {
                            result = "Företagsnamn får max vara 50 tecken.";
                        }
                        break;

                    case nameof(OrganisationNumber):
                        if (string.IsNullOrEmpty(OrganisationNumber))
                        {
                            result = "Organisationsnummer får inte vara tomt.";
                        }
                        else if (OrganisationNumber.Length != 11 || !System.Text.RegularExpressions.Regex.IsMatch(OrganisationNumber.Replace("-", ""), @"^\d{10}$"))
                        {
                            result = "Organisationsnummer måste ha formatet XXXXXX-XXXX och endast innehålla siffror.";
                        }
                        else if (_businessCustomerController.IsOrganisationNumberExists(OrganisationNumber))
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

                    case nameof(Email):
                        if (!string.IsNullOrEmpty(Email) && !Email.Contains("@"))
                        {
                            result = "Mejl måste innehålla '@'.";
                        }
                        break;

                    case nameof(City):
                        if (string.IsNullOrEmpty(City))
                        {
                            result = "Stad får inte vara tom.";
                        }
                        break;

                    case nameof(AreaCode):
                        if (string.IsNullOrEmpty(AreaCode))
                        {
                            result = "Riktnummer får inte vara tomt.";
                        }
                        break;

                    case nameof(PhoneNumber):
                        if (string.IsNullOrEmpty(PhoneNumber))
                        {
                            result = "Telefonnummer får inte vara tomt.";
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
                nameof(CompanyName),
                nameof(OrganisationNumber),
                nameof(StreetAddress),
                nameof(ZipCode),
                nameof(City),
                nameof(AreaCode),
                nameof(PhoneNumber),
                nameof(email)
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

            BusinessCustomer businessCustomer = new BusinessCustomer
            {
                CompanyName = CompanyName,
                OrganisationNumber = OrganisationNumber,
                StreetAddress = StreetAddress,
                ZipCode = ZipCode,
                City = City,
                AreaCode = AreaCode,
                PhoneNumber = PhoneNumber,
                Email = Email
            };

            _businessCustomerController.AddBusinessCustomer(businessCustomer);
            ValidationError = "Kund tillagd!";
            IsSnackbarActive = true;
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
