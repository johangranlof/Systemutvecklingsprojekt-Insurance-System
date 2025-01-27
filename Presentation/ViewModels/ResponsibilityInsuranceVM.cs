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
    public class ResponsibilityInsuranceVM : BaseVM, IDataErrorInfo
    {
        private readonly MainVM _mainViewModel;
        private readonly BusinessInsuranceController _businessInsuranceController;
        private readonly BusinessCustomerController _businessCustomerController;
        private readonly PrivateCustomerController _privateCustomerController;
        private readonly EmployeeController _employeeController;
        private const int Prisbasbelopp2024 = 57300;
        private bool _isSnackbarActive;
        private string _snackbarMessage;
        private bool isSaveAttempted = false;
        public ResponsibilityInsuranceVM(MainVM mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _businessCustomerController = new BusinessCustomerController();
            _businessInsuranceController = new BusinessInsuranceController();
            _privateCustomerController = new PrivateCustomerController();
            _employeeController = new EmployeeController();
            LoadBusinessCustomers();
            ExtentOptions = new ObservableCollection<int> { 3000000, 5000000, 10000000 };
            LiabilityAmounts = new ObservableCollection<string> { "1/4 prisbasbelopp", "1/2 prisbasbelopp", "3/4 prisbasbelopp", "1 prisbasbelopp" };

            ValidationError = string.Empty;
        }

        public ObservableCollection<string> BusinessCustomerName
        {
            get => _businessCustomerName;
            set
            {
                _businessCustomerName = value;
                OnPropertyChanged(nameof(BusinessCustomerName));
            }
        }
        private ObservableCollection<string> _businessCustomerName;

        public ObservableCollection<int> ExtentOptions
        {
            get => _extentOptions;
            set
            {
                _extentOptions = value;
                OnPropertyChanged(nameof(ExtentOptions));
            }
        }
        private ObservableCollection<int> _extentOptions;

        public ObservableCollection<string> LiabilityAmounts
        {
            get => _liabilityAmounts;
            set
            {
                _liabilityAmounts = value;
                OnPropertyChanged(nameof(LiabilityAmounts));
            }
        }
        private ObservableCollection<string> _liabilityAmounts;

        public ObservableCollection<BusinessCustomer> AllBusinessCustomers
        {
            get => _allBusinessCustomers;
            set
            {
                _allBusinessCustomers = value;
                OnPropertyChanged(nameof(AllBusinessCustomers));
            }
        }
        private ObservableCollection<BusinessCustomer> _allBusinessCustomers;

        public ObservableCollection<PaymentPlan> PaymentPlans
        {
            get => _paymentPlans;
            set
            {
                _paymentPlans = value;
                OnPropertyChanged(nameof(PaymentPlans));
            }
        }
        private ObservableCollection<PaymentPlan> _paymentPlans;

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

        public string ValidationError
        {
            get => _validationError;
            set
            {
                _validationError = value;
                OnPropertyChanged(nameof(ValidationError));
            }
        }
        private string _validationError;

        public int SelectedExtent
        {
            get => _selectedExtent;
            set
            {
                _selectedExtent = value;
                OnPropertyChanged(nameof(SelectedExtent));
            }
        }
        private int _selectedExtent;

        public string SelectedLiabilityAmount
        {
            get => _selectedLiabilityAmount;
            set
            {
                _selectedLiabilityAmount = value;
                OnPropertyChanged(nameof(SelectedLiabilityAmount));
            }
        }
        private string _selectedLiabilityAmount;

        public string Note
        {
            get => _note;
            set
            {
                _note = value;
                OnPropertyChanged(nameof(Note));
            }
        }
        private string _note;

        public PaymentPlan SelectedPaymentPlan
        {
            get => _selectedPaymentPlan;
            set
            {
                _selectedPaymentPlan = value;
                OnPropertyChanged(nameof(SelectedPaymentPlan));
            }
        }
        private PaymentPlan _selectedPaymentPlan;

        public string SelectedBusinessCustomerName
        {
            get => _selectedBusinessCustomerName;
            set
            {
                _selectedBusinessCustomerName = value;
                SelectedBusinessCustomer = AllBusinessCustomers.FirstOrDefault(pc => $"{pc.CompanyName} {pc.OrganisationNumber}" == value);
                OnPropertyChanged(nameof(SelectedBusinessCustomerName));
            }
        }
        private string _selectedBusinessCustomerName;

        public BusinessCustomer SelectedBusinessCustomer
        {
            get => _selectedBusinessCustomer;
            set
            {
                _selectedBusinessCustomer = value;
                OnPropertyChanged(nameof(SelectedBusinessCustomer));
            }
        }
        private BusinessCustomer _selectedBusinessCustomer;

        public string ContactFirstName
        {
            get => _contactFirstName;
            set
            {
                _contactFirstName = value;
                OnPropertyChanged(nameof(ContactFirstName));
            }
        }
        private string _contactFirstName;

        public string ContactLastName
        {
            get => _contactLastName;
            set
            {
                _contactLastName = value;
                OnPropertyChanged(nameof(ContactLastName));
            }
        }
        private string _contactLastName;

        public string ContactPhoneNumber
        {
            get => _contactPhoneNumber;
            set
            {
                _contactPhoneNumber = value;
                OnPropertyChanged(nameof(ContactPhoneNumber));
            }
        }
        private string _contactPhoneNumber;

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(async () => await Save());

        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new RelayCommand(() =>
        {
            _mainViewModel.CurrentView = new HandleInsurancesUC
            {
                DataContext = new HandleInsurancesVM(_mainViewModel)
            };
        });

        private void LoadBusinessCustomers()
        {
            var businessCustomers = _businessCustomerController.GetBusinessCustomers();
            var businessCustomerNames = businessCustomers.Select(pc => $"{pc.CompanyName} {pc.OrganisationNumber}").ToList();
            var paymentPlans = _privateCustomerController.GetPaymentPlan().ToList();

            AllBusinessCustomers = new ObservableCollection<BusinessCustomer>(businessCustomers);
            BusinessCustomerName = new ObservableCollection<string>(businessCustomerNames);
            PaymentPlans = new ObservableCollection<PaymentPlan>(paymentPlans);
        }

        private int CalculateDeductible()
        {
            return SelectedLiabilityAmount switch
            {
                "1/4 prisbasbelopp" => Prisbasbelopp2024 / 4,
                "1/2 prisbasbelopp" => Prisbasbelopp2024 / 2,
                "3/4 prisbasbelopp" => (Prisbasbelopp2024 * 3) / 4,
                "1 prisbasbelopp" => Prisbasbelopp2024,
                _ => throw new Exception("Ogiltigt självriskalternativ valt.")
            };
        }

        private int CalculatePremium()
        {
            int deductible = CalculateDeductible();

            return SelectedExtent switch
            {
                3000000 => deductible switch
                {
                    (Prisbasbelopp2024 / 4) => 800,
                    (Prisbasbelopp2024 / 2) => 600,
                    (Prisbasbelopp2024 * 3 / 4) => 700,
                    Prisbasbelopp2024 => 500,
                    _ => throw new Exception("Ogiltigt självriskalternativ valt.")
                },
                5000000 => deductible switch
                {
                    (Prisbasbelopp2024 / 4) => 1300,
                    (Prisbasbelopp2024 / 2) => 1200,
                    (Prisbasbelopp2024 * 3 / 4) => 1100,
                    Prisbasbelopp2024 => 900,
                    _ => throw new Exception("Ogiltigt självriskalternativ valt.")
                },
                10000000 => deductible switch
                {
                    (Prisbasbelopp2024 / 4) => 1800,
                    (Prisbasbelopp2024 / 2) => 1700,
                    (Prisbasbelopp2024 * 3 / 4) => 1600,
                    Prisbasbelopp2024 => 1400,
                    _ => throw new Exception("Ogiltigt självriskalternativ valt.")
                },
                _ => throw new Exception("Ogiltig omfattning vald.")
            };
        }

        private DateTime CalculateNextPaymentDate(DateTime startDate, string paymentPlanType)
        {
            return paymentPlanType switch
            {
                "Månad" => startDate.AddMonths(1),
                "Kvartal" => startDate.AddMonths(3),
                "Halvår" => startDate.AddMonths(6),
                "Helår" => startDate.AddYears(1),
                _ => throw new Exception("Ogiltig betalningsplanstyp.")
            };
        }

        private async Task Save()
        {
            isSaveAttempted = true;
            var validationFields = new List<string>
            {
                nameof(SelectedBusinessCustomerName),
                nameof(SelectedPaymentPlan),
                nameof(ContactFirstName),
                nameof(ContactLastName),
                nameof(ContactPhoneNumber),
                nameof(SelectedExtent),
                nameof(SelectedLiabilityAmount)
            };
            string validationError = string.Empty;

            foreach (var field in validationFields)
            {
                string fieldError = this[field];
                if (!string.IsNullOrEmpty(fieldError))
                {
                    validationError += fieldError + "\n";
                    OnPropertyChanged(field);
                }
            }

            if (!string.IsNullOrEmpty(validationError))
            {
                SnackbarMessage = validationError;
                IsSnackbarActive = true;
                return;
            }

            try
            {
                var userSession = UserSession.GetInstance();
                var currentUser = userSession.User;
                int agentNumber = _employeeController.GetAgentNumberByEmployeeNumber(currentUser.EmployeeNumber);

                DateTime nextPaymentDate = CalculateNextPaymentDate(DateTime.Now, SelectedPaymentPlan.Type);
                string insuranceNumber = GenerateUniqueInsuranceNumber("FA");

                var liability = new LiabilityInsurance
                {
                    ContactFirstName = ContactFirstName,
                    ContactLastName = ContactLastName,
                    ContactPhoneNumber = ContactPhoneNumber,
                    InsuranceNumber = insuranceNumber,
                    Note = Note,
                    Deductible = CalculateDeductible(),
                    Premium = CalculatePremium(),
                    AgentNumber = agentNumber,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1),
                    BusinessCustomerId = SelectedBusinessCustomer?.Id ?? 0,
                    Extent = SelectedExtent,
                    PaymentPlanId = SelectedPaymentPlan.Id,
                    InsuranceStatusId = 3,
                    UserId = currentUser.Id,
                    NextPaymentDate = nextPaymentDate
                };

                _businessInsuranceController.AddBusinessInsurance(liability);

                SnackbarMessage = "Försäkring tillagd!";
                IsSnackbarActive = true;
                await Task.Delay(3000); //snackbar stannar kvar 3 sekunder
                IsSnackbarActive = false;
            }
            catch
            {
                SnackbarMessage = "Ett fel uppstod vid sparandet av försäkringen";
                IsSnackbarActive = true;
                await Task.Delay(3000); //snackbar stannar kvar 3 sekunder
                IsSnackbarActive = false;
            }
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

        // Returnerar felmeddelande för fält beroende på valideringstillstånd
        public string this[string columnName]
        {
            get
            {
                if (!isSaveAttempted) return string.Empty;

                return columnName switch
                {
                    nameof(SelectedBusinessCustomerName) when SelectedBusinessCustomer == null => "Vänligen välj en giltig företagskund.",
                    nameof(SelectedPaymentPlan) when SelectedPaymentPlan == null => "Vänligen välj en giltig betalningsplan.",
                    nameof(ContactFirstName) when string.IsNullOrWhiteSpace(ContactFirstName) => "Kontakts förnamn får inte vara tomt.",
                    nameof(ContactLastName) when string.IsNullOrWhiteSpace(ContactLastName) => "Kontakts efternamn får inte vara tomt.",
                    nameof(ContactPhoneNumber) when string.IsNullOrWhiteSpace(ContactPhoneNumber) => "Kontakts telefonnummer får inte vara tomt.",
                    nameof(SelectedExtent) when SelectedExtent <= 0 => "Vänligen välj ett giltigt omfattningsbelopp.",
                    nameof(SelectedLiabilityAmount) when string.IsNullOrWhiteSpace(SelectedLiabilityAmount) => "Vänligen välj ett giltigt självriskbelopp.",
                    _ => string.Empty
                };
            }
        }

        public string Error => string.Empty;
    }
}




