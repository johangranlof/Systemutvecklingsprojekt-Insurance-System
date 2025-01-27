using Business.Controllers;
using Business.Utilities;
using Entities;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class OfferCarInsuranceVM : MainVM, IDataErrorInfo
    {
        private readonly MainVM _mainViewModel;
        private readonly BusinessInsuranceController _businessInsuranceController;
        private readonly BusinessCustomerController _businessCustomerController;
        private readonly PrivateCustomerController _privateCustomerController;
        private readonly EmployeeController _employeeController;
        private bool _isSnackbarActive;
        private string _snackbarMessage;
        private bool isSaveAttempted = false;

        public OfferCarInsuranceVM(MainVM mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _businessCustomerController = new BusinessCustomerController();
            _businessInsuranceController = new BusinessInsuranceController();
            _privateCustomerController = new PrivateCustomerController();
            _employeeController = new EmployeeController();

            _liabilityAmounts = new ObservableCollection<int> { 1000, 2000, 3500 };
            _coverageOptions = new ObservableCollection<string> { "Trafikförsäkring", "Halvförsäkring", "Helförsäkring" };

            LoadBusinessCustomers();
            LoadCities();

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

        public string SnackbarMessage
        {
            get => _snackbarMessage;
            set
            {
                _snackbarMessage = value;
                OnPropertyChanged(nameof(SnackbarMessage));
            }
        }

        private ObservableCollection<int> _liabilityAmounts;
        public ObservableCollection<int> LiabilityAmounts
        {
            get { return _liabilityAmounts; }
            set
            {
                _liabilityAmounts = value;
                OnPropertyChanged(nameof(LiabilityAmounts));
            }
        }

        private int _selectedLiabilityAmount;
        public int SelectedLiabilityAmount
        {
            get { return _selectedLiabilityAmount; }
            set
            {
                _selectedLiabilityAmount = value;
                OnPropertyChanged(nameof(SelectedLiabilityAmount));
            }
        }

        private ObservableCollection<City> _cities;
        public ObservableCollection<City> Cities
        {
            get { return _cities; }
            set
            {
                _cities = value;
                OnPropertyChanged(nameof(Cities));
            }
        }

        private City _selectedCity;
        public City SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                _selectedCity = value;
                OnPropertyChanged(nameof(SelectedCity));
            }
        }

        private ObservableCollection<string> _businessCustomerName;
        public ObservableCollection<string> BusinessCustomerName
        {
            get { return _businessCustomerName; }
            set
            {
                _businessCustomerName = value;
                OnPropertyChanged(nameof(BusinessCustomerName));
            }
        }

        private ObservableCollection<string> _coverageOptions;
        public ObservableCollection<string> CoverageOptions
        {
            get { return _coverageOptions; }
            set
            {
                _coverageOptions = value;
                OnPropertyChanged(nameof(CoverageOptions));
            }
        }

        private string _selectedCoverage;
        public string SelectedCoverage
        {
            get { return _selectedCoverage; }
            set
            {
                _selectedCoverage = value;
                OnPropertyChanged(nameof(SelectedCoverage));
            }
        }

        private string _note;
        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                OnPropertyChanged(nameof(Note));
            }
        }

        private ObservableCollection<BusinessCustomer> _allBusinessCustomers;
        public ObservableCollection<BusinessCustomer> AllBusinessCustomers
        {
            get { return _allBusinessCustomers; }
            set
            {
                _allBusinessCustomers = value;
                OnPropertyChanged(nameof(AllBusinessCustomers));
            }
        }

        private ObservableCollection<PaymentPlan> _paymentPlans;
        public ObservableCollection<PaymentPlan> PaymentPlans
        {
            get { return _paymentPlans; }
            set
            {
                _paymentPlans = value;
                OnPropertyChanged(nameof(PaymentPlans));
            }
        }

        private PaymentPlan _selectedPaymentPlan;
        public PaymentPlan SelectedPaymentPlan
        {
            get { return _selectedPaymentPlan; }
            set
            {
                _selectedPaymentPlan = value;
                OnPropertyChanged(nameof(SelectedPaymentPlan));
            }
        }

        private string _selectedBusinessCustomerName;
        public string SelectedBusinessCustomerName
        {
            get { return _selectedBusinessCustomerName; }
            set
            {
                _selectedBusinessCustomerName = value;
                SelectedBusinessCustomer = AllBusinessCustomers.FirstOrDefault(pc => $"{pc.CompanyName} {pc.OrganisationNumber}" == value);
                OnPropertyChanged(nameof(SelectedBusinessCustomerName));
            }
        }

        private BusinessCustomer _selectedBusinessCustomer;
        public BusinessCustomer SelectedBusinessCustomer
        {
            get { return _selectedBusinessCustomer; }
            set
            {
                _selectedBusinessCustomer = value;
                OnPropertyChanged(nameof(SelectedBusinessCustomer));
            }
        }

        private string _driverFirstName;
        public string DriverFirstName
        {
            get { return _driverFirstName; }
            set
            {
                _driverFirstName = value;
                OnPropertyChanged(nameof(DriverFirstName));
            }
        }

        private string _driverLastName;
        public string DriverLastName
        {
            get { return _driverLastName; }
            set
            {
                _driverLastName = value;
                OnPropertyChanged(nameof(DriverLastName));
            }
        }

        private string _driverSSN;
        public string DriverSSN
        {
            get { return _driverSSN; }
            set
            {
                _driverSSN = value;
                OnPropertyChanged(nameof(DriverSSN));
            }
        }

        private string _registrationNumber;
        public string RegistrationNumber
        {
            get { return _registrationNumber; }
            set
            {
                _registrationNumber = value;
                OnPropertyChanged(nameof(RegistrationNumber));
            }
        }

        private string _contactFirstName;
        public string ContactFirstName
        {
            get { return _contactFirstName; }
            set
            {
                _contactFirstName = value;
                OnPropertyChanged(nameof(ContactFirstName));
            }
        }

        private string _contactLastName;
        public string ContactLastName
        {
            get { return _contactLastName; }
            set
            {
                _contactLastName = value;
                OnPropertyChanged(nameof(ContactLastName));
            }
        }

        private string _contactPhoneNumber;
        public string ContactPhoneNumber
        {
            get { return _contactPhoneNumber; }
            set
            {
                _contactPhoneNumber = value;
                OnPropertyChanged(nameof(ContactPhoneNumber));
            }
        }

        private void LoadCities()
        {
            var cities = _businessInsuranceController.GetAllCities();
            Cities = new ObservableCollection<City>(cities);
        }

        private void LoadBusinessCustomers()
        {
            var businessCustomers = _businessCustomerController.GetBusinessCustomers();
            var businessCustomerNames = businessCustomers.Select(pc => $"{pc.CompanyName} {pc.OrganisationNumber}").ToList();
            var paymentPlans = _privateCustomerController.GetPaymentPlan().ToList();

            AllBusinessCustomers = new ObservableCollection<BusinessCustomer>(businessCustomers);
            BusinessCustomerName = new ObservableCollection<string>(businessCustomerNames);
            PaymentPlans = new ObservableCollection<PaymentPlan>(paymentPlans);
        }

        private string GenerateUniqueInsuranceNumber(string prefix)
        {
            Random random = new Random();
            string insuranceNumber;

            do
            {
                int randomNumber = random.Next(0000, 9999);
                insuranceNumber = $"{prefix}-{randomNumber}";
            }
            while (_businessInsuranceController.IsInsuranceNumberExists(insuranceNumber));

            return insuranceNumber;
        }

        private void Save()
        {
            isSaveAttempted = true;

            var validationFields = new List<string>
            {
                nameof(SelectedBusinessCustomerName),
                nameof(SelectedPaymentPlan),
                nameof(DriverFirstName),
                nameof(DriverLastName),
                nameof(DriverSSN),
                nameof(ContactFirstName),
                nameof(ContactLastName),
                nameof(ContactPhoneNumber),
                nameof(RegistrationNumber),
                nameof(SelectedCity),
                nameof(SelectedCoverage),
                nameof(SelectedLiabilityAmount)
            };

            string validationError = string.Empty;

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
                SetSnackbarMessage(validationError);
                return;
            }

            try
            {
                var userSession = UserSession.GetInstance();
                var currentUser = userSession.User;

                int agentNumber = _employeeController.GetAgentNumberByEmployeeNumber(currentUser.EmployeeNumber);

                int extentValue = CoverageToExtent(SelectedCoverage);

                int baseCost = GetBaseCost(SelectedLiabilityAmount, SelectedCoverage);
                int zoneFactor = (int)GetZoneFactor(SelectedCity.ZoneId);
                int finalPremium = baseCost * zoneFactor;

                DateTime nextPaymentDate = CalculateNextPaymentDate(DateTime.Now, SelectedPaymentPlan.Type);

                string insuranceNumber = GenerateUniqueInsuranceNumber("FF"); // Fordonförsäkring

                var newVehicle = new VehicleInsurance
                {
                    InsuranceNumber = insuranceNumber,
                    ContactFirstName = ContactFirstName,
                    ContactLastName = ContactLastName,
                    ContactPhoneNumber = ContactPhoneNumber,
                    AgentNumber = agentNumber,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1),
                    BusinessCustomerId = SelectedBusinessCustomer.Id,
                    NextPaymentDate = nextPaymentDate,
                    Premium = finalPremium,
                    PaymentPlanId = SelectedPaymentPlan.Id,
                    InsuranceStatusId = 3,
                    UserId = currentUser.Id,
                    RegNumber = RegistrationNumber,
                    DriverFirstName = DriverFirstName,
                    DriverLastName = DriverLastName,
                    DriverSSN = DriverSSN,
                    Debuctible = SelectedLiabilityAmount,
                    Extent = extentValue,
                    ZoneId = SelectedCity.ZoneId,
                };

                _businessInsuranceController.AddVehicleInsurance(newVehicle);

                SetSnackbarMessage("Försäkring tillagd!");
            }
            catch (Exception ex)
            {
                SetSnackbarMessage($"Ett fel uppstod vid sparandet av försäkringen: {ex.Message}");
            }
        }

        private void SetSnackbarMessage(string message)
        {
            SnackbarMessage = message;
            IsSnackbarActive = true;
        }

        private void NotifyValidation(string propertyName)
        {
            OnPropertyChanged(propertyName);
            ValidationError = this[propertyName];
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
                    case nameof(SelectedBusinessCustomerName):
                        if (SelectedBusinessCustomer == null)
                        {
                            result = "Vänligen välj en giltig företagskund.";
                        }
                        break;

                    case nameof(SelectedPaymentPlan):
                        if (SelectedPaymentPlan == null)
                        {
                            result = "Vänligen välj en giltig betalningsplan.";
                        }
                        break;

                    case nameof(DriverFirstName):
                        if (string.IsNullOrWhiteSpace(DriverFirstName))
                        {
                            result = "Förarens förnamn får inte vara tomt.";
                        }
                        break;

                    case nameof(DriverLastName):
                        if (string.IsNullOrWhiteSpace(DriverLastName))
                        {
                            result = "Förarens efternamn får inte vara tomt.";
                        }
                        break;

                    case nameof(DriverSSN):
                        if (string.IsNullOrEmpty(DriverSSN))
                        {
                            result = "Personnummer får inte vara tomt.";
                        }
                        else if (DriverSSN.Length != 10 || !System.Text.RegularExpressions.Regex.IsMatch(DriverSSN, @"^\d{10}$"))
                        {
                            result = "Personnummer måste innehålla 10 siffror.";
                        }
                        break;

                    case nameof(ContactFirstName):
                        if (string.IsNullOrWhiteSpace(ContactFirstName))
                        {
                            result = "Kontaktpersonens förnamn får inte vara tomt.";
                        }
                        break;

                    case nameof(ContactLastName):
                        if (string.IsNullOrWhiteSpace(ContactLastName))
                        {
                            result = "Kontaktpersonens efternamn får inte vara tomt.";
                        }
                        break;

                    case nameof(ContactPhoneNumber):
                        if (string.IsNullOrWhiteSpace(ContactPhoneNumber))
                        {
                            result = "Kontaktpersonens telefonnummer får inte vara tomt.";
                        }
                        break;

                    case nameof(RegistrationNumber):
                        if (string.IsNullOrWhiteSpace(RegistrationNumber))
                        {
                            result = "Registreringsnummer får inte vara tomt.";
                        }
                        break;

                    case nameof(SelectedCity):
                        if (SelectedCity == null)
                        {
                            result = "Vänligen välj en giltig stad.";
                        }
                        break;

                    case nameof(SelectedCoverage):
                        if (string.IsNullOrWhiteSpace(SelectedCoverage))
                        {
                            result = "Vänligen välj en försäkringsomfattning.";
                        }
                        break;

                    case nameof(SelectedLiabilityAmount):
                        if (SelectedLiabilityAmount <= 0)
                        {
                            result = "Vänligen välj en giltig självrisk.";
                        }
                        break;
                }

                return result;
            }
        }

        public string Error => string.Empty;

        private int CoverageToExtent(string coverage)
        {
            switch (coverage.ToLower())
            {
                case "trafikförsäkring":
                    return 1;
                case "halvförsäkring":
                    return 2;
                case "helförsäkring":
                    return 3;
                default:
                    return 0;
            }
        }

        private int GetBaseCost(int deductible, string coverage)
        {
            switch (deductible)
            {
                case 1000:
                    return coverage.ToLower() switch
                    {
                        "trafikförsäkring" => 350,
                        "halvförsäkring" => 550,
                        "helförsäkring" => 800,
                        _ => 0
                    };
                case 2000:
                    return coverage.ToLower() switch
                    {
                        "trafikförsäkring" => 300,
                        "halvförsäkring" => 450,
                        "helförsäkring" => 700,
                        _ => 0
                    };
                case 3500:
                    return coverage.ToLower() switch
                    {
                        "trafikförsäkring" => 250,
                        "halvförsäkring" => 400,
                        "helförsäkring" => 600,
                        _ => 0
                    };
                default:
                    return 0;
            }
        }

        private DateTime CalculateNextPaymentDate(DateTime startDate, string paymentPlanType)
        {
            DateTime nextPaymentDate = startDate;

            switch (paymentPlanType.ToLower())
            {
                case "månad":
                    nextPaymentDate = startDate.AddMonths(1);
                    break;
                case "kvartal":
                    nextPaymentDate = startDate.AddMonths(3);
                    break;
                case "halvår":
                    nextPaymentDate = startDate.AddMonths(6);
                    break;
                case "helår":
                    nextPaymentDate = startDate.AddYears(1);
                    break;
            }

            return nextPaymentDate;
        }

        private double GetZoneFactor(int zoneId)
        {
            switch (zoneId)
            {
                case 1:
                    return 1.3;
                case 2:
                    return 1.2;
                case 3:
                    return 1.1;
                case 4:
                    return 1.0;
                default:
                    return 1.0;
            }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(() => Save());

        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new RelayCommand(() =>
        {
            _mainViewModel.CurrentView = new HandleInsurancesUC
            {
                DataContext = new HandleInsurancesVM(_mainViewModel)
            };
        });
    }
}
