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
    public class ViewBusinessCustomerVM : BaseVM, IDataErrorInfo
    {
        private readonly MainVM _mainViewModel;
        private readonly EmployeeController _employeeController;
        public BusinessCustomer BusinessCustomer { get; set; }
        private bool isSaveAttempted = false;
        private readonly BusinessCustomerController _businessCustomerController;
        private bool _isSnackbarActive;

        public ViewBusinessCustomerVM(MainVM mainViewModel, BusinessCustomer businessCustomer)
        {
            _mainViewModel = mainViewModel;
            BusinessCustomer = businessCustomer;
            _businessCustomerController = new BusinessCustomerController();
            _employeeController = new EmployeeController();
            BusinessCustomer.CustomerProspectInformation = _businessCustomerController.GetCustomerProspectInformation(BusinessCustomer.Id);
            ValidationError = string.Empty;
            CompanyName = BusinessCustomer.CompanyName;
            StreetAddress = BusinessCustomer.StreetAddress;
            ZipCode = BusinessCustomer.ZipCode;
            City = BusinessCustomer.City;
            AreaCode = BusinessCustomer.AreaCode;
            PhoneNumber = BusinessCustomer.PhoneNumber;
            Email = BusinessCustomer.Email;
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
            if (_businessCustomerController.HasActiveInsurance(BusinessCustomer))
            {
                _removeActive = false;
                return;
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

        private string companyName;
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


        private void UpdateBusinesscustomer()
        {
            isSaveAttempted = true;

            string validationError = string.Empty;
            var validationFields = new List<string>
            {
                nameof(CompanyName),

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

            BusinessCustomer.CompanyName = CompanyName;
            BusinessCustomer.StreetAddress = StreetAddress;
            BusinessCustomer.ZipCode = ZipCode;
            BusinessCustomer.City = City;
            BusinessCustomer.AreaCode = AreaCode;
            BusinessCustomer.PhoneNumber = PhoneNumber;
            BusinessCustomer.Email = Email;
            _businessCustomerController.UpdateBusinessCustomer(BusinessCustomer);
            SnackbarMessage = "Kund har uppdaterats";
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


        private ICommand _updateBusinessCustomerCommand;
        public ICommand UpdateBusinesscustomerCommand => _updateBusinessCustomerCommand ??= new RelayCommand(() =>
        {
            UpdateBusinesscustomer();

        });

        private ICommand _removeBusinessCustomerPopUpCommand;
        public ICommand RemoveBusinessCustomerPopUpCommand => _removeBusinessCustomerPopUpCommand ??= new RelayCommand(() =>
        {

            try
            {


                _businessCustomerController.DeleteBusinessCustomer(BusinessCustomer);
            }
            catch (Exception ex)
            {
                SnackbarMessage = "Ett fel uppstod vid borttagning av kunden";
            }

        });


        private ICommand _removePopUpCommand;
        public ICommand RemovePopUpCommand => _removePopUpCommand ??= new RelayCommand(() =>
        {
            _mainViewModel.IsOverlayVisible = true;
            var removeCustomerPopup = new RemoveCustomerPopup();
            var removeCustomerVM = new RemoveCustomerVM(BusinessCustomer, removeCustomerPopup, _mainViewModel, _businessCustomerController);

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
                CustomerId = BusinessCustomer.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName
            };
            _businessCustomerController.AddProspectInformation(prospectInfo);
            BusinessCustomer.CustomerProspectInformation.Add(prospectInfo);
        }

        private ICommand _handleProspectCommand;
        public ICommand HandleProspectCommand => _handleProspectCommand ??= new RelayCommand(() =>
        {
            if (!SellerFunctions) return;
            OpenProspectInfoPopup();
        });


    }
}
