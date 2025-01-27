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
    public class Sickness_and_accident_insuranceVM : BaseVM, IDataErrorInfo
    {
        private readonly MainVM _mainViewModel;
        private readonly PrivateInsuranceController _privateInsuranceController;
        private readonly PrivateCustomerController _privateCustomerController;
        private readonly EmployeeController _employeeController;
        private bool _isSnackbarActive;
        private bool isSaveAttempted = false;

        public Sickness_and_accident_insuranceVM(MainVM mainViewModel)
        {
            _privateInsuranceController = new PrivateInsuranceController();
            _mainViewModel = mainViewModel;
            _privateCustomerController = new PrivateCustomerController();
            _employeeController = new EmployeeController();
            _insuranceAmounts = new ObservableCollection<int> { 350000, 450000, 550000 };
            _longTermSicknessAmounts = new ObservableCollection<int> { 500, 1000, 1500, 2000, 2500, 3000, 3500, 4000 };
            _accidentDisabilityAmounts = new ObservableCollection<int> { 100000, 200000, 300000, 400000, 500000, 600000, 700000, 800000 };

            LoadPrivateCustomers();
        }

        private ObservableCollection<int> _insuranceAmounts;
        public ObservableCollection<int> InsuranceAmounts
        {
            get => _insuranceAmounts;
            set
            {
                _insuranceAmounts = value;
                OnPropertyChanged(nameof(InsuranceAmounts));
            }
        }

        private ObservableCollection<int> _longTermSicknessAmounts;
        public ObservableCollection<int> LongTermSicknessAmounts
        {
            get => _longTermSicknessAmounts;
            set
            {
                _longTermSicknessAmounts = value;
                OnPropertyChanged(nameof(LongTermSicknessAmounts));
            }
        }

        private ObservableCollection<int> _accidentDisabilityAmounts;
        public ObservableCollection<int> AccidentDisabilityAmounts
        {
            get => _accidentDisabilityAmounts;
            set
            {
                _accidentDisabilityAmounts = value;
                OnPropertyChanged(nameof(AccidentDisabilityAmounts));
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

        private bool _isLongSicknessChosen;
        public bool IsLongSicknessChosen
        {
            get => _isLongSicknessChosen;
            set
            {
                _isLongSicknessChosen = value;
                OnPropertyChanged(nameof(IsLongSicknessChosen));
            }
        }

        private bool _isInvalidityChosen;
        public bool IsInvalidityChosen
        {
            get => _isInvalidityChosen;
            set
            {
                _isInvalidityChosen = value;
                OnPropertyChanged(nameof(IsInvalidityChosen));
            }
        }

        private int _selectedAccidentDisabilityAmount;
        public int SelectedAccidentDisabilityAmount
        {
            get => _selectedAccidentDisabilityAmount;
            set
            {
                _selectedAccidentDisabilityAmount = value;
                OnPropertyChanged(nameof(SelectedAccidentDisabilityAmount));
            }
        }

        private ObservableCollection<string> _privateCustomerName;
        public ObservableCollection<string> PrivateCustomerName
        {
            get => _privateCustomerName;
            set
            {
                _privateCustomerName = value;
                OnPropertyChanged(nameof(PrivateCustomerName));
            }
        }


        private string _selectedPrivateCustomerName;
        public string SelectedPrivateCustomerName
        {
            get => _selectedPrivateCustomerName;
            set
            {
                _selectedPrivateCustomerName = value;
                SelectedPrivateCustomer = AllPrivateCustomers.FirstOrDefault(pc => $"{pc.FirstName} {pc.LastName} {pc.SSN}" == value);
                OnPropertyChanged(nameof(SelectedPrivateCustomerName));
            }
        }

        private ObservableCollection<PaymentPlan> _paymentPlans;
        public ObservableCollection<PaymentPlan> PaymentPlans
        {
            get => _paymentPlans;
            set
            {
                _paymentPlans = value;
                OnPropertyChanged(nameof(PaymentPlans));
            }
        }

        private PaymentPlan _selectedPaymentPlan;
        public PaymentPlan SelectedPaymentPlan
        {
            get => _selectedPaymentPlan;
            set
            {
                _selectedPaymentPlan = value;
                OnPropertyChanged(nameof(SelectedPaymentPlan));
            }
        }

        private PrivateCustomer _selectedPrivateCustomer;
        public PrivateCustomer SelectedPrivateCustomer
        {
            get => _selectedPrivateCustomer;
            set
            {
                _selectedPrivateCustomer = value;
                OnPropertyChanged(nameof(SelectedPrivateCustomer));
            }
        }

        private int _selectedInsuranceAmount;
        public int SelectedInsuranceAmount
        {
            get => _selectedInsuranceAmount;
            set
            {
                _selectedInsuranceAmount = value;
                OnPropertyChanged(nameof(SelectedInsuranceAmount));
            }
        }

        private int _selectedLongTermSicknessAmount;
        public int SelectedLongTermSicknessAmount
        {
            get => _selectedLongTermSicknessAmount;
            set
            {
                _selectedLongTermSicknessAmount = value;
                OnPropertyChanged(nameof(SelectedLongTermSicknessAmount));
            }
        }

        private int _monthlyPayment;
        public int MonthlyPayment
        {
            get => _monthlyPayment;
            set
            {
                _monthlyPayment = value;
                OnPropertyChanged(nameof(MonthlyPayment));
            }
        }

        private ObservableCollection<PrivateCustomer> _allPrivateCustomers;
        public ObservableCollection<PrivateCustomer> AllPrivateCustomers
        {
            get => _allPrivateCustomers;
            set
            {
                _allPrivateCustomers = value;
                OnPropertyChanged(nameof(AllPrivateCustomers));
            }
        }

        private string _notes;
        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
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
            while (_privateInsuranceController.IsInsuranceNumberExists(insuranceNumber));
            return insuranceNumber;
        }

        private void LoadPrivateCustomers()
        {
            var privateCustomers = _privateCustomerController.GetPrivateCustomers();
            var privateCustomerNames = privateCustomers.Select(pc => $"{pc.FirstName} {pc.LastName} {pc.SSN}").ToList();
            var paymentPlans = _privateCustomerController.GetPaymentPlan().ToList();

            PrivateCustomerName = new ObservableCollection<string>(privateCustomerNames);
            PaymentPlans = new ObservableCollection<PaymentPlan>(paymentPlans);
            AllPrivateCustomers = new ObservableCollection<PrivateCustomer>(privateCustomers);
        }

        private DateTime CalculateNextPaymentDate(DateTime startDate, string paymentPlanType)
        {
            return paymentPlanType switch
            {
                "Månad" => startDate.AddMonths(1),
                "Kvartal" => startDate.AddMonths(3),
                "Halvår" => startDate.AddMonths(6),
                "Helår" => startDate.AddYears(1),
                _ => startDate,
            };
        }

        public int CalculateMonthlyPremium()
        {
            MonthlyPayment = SelectedInsuranceAmount switch
            {
                350000 => 175,
                450000 => 225,
                550000 => 275,
                _ => 0,
            };
            return MonthlyPayment;
        }

        public void SavePrivateInsurance()
        {
            isSaveAttempted = true;
            OnPropertyChanged(nameof(SelectedPrivateCustomerName));
            OnPropertyChanged(nameof(SelectedPaymentPlan));
            OnPropertyChanged(nameof(SelectedLongTermSicknessAmount));
            OnPropertyChanged(nameof(SelectedAccidentDisabilityAmount));
            OnPropertyChanged(nameof(SelectedInsuranceAmount));


            string validationError = string.Empty;
            var validationFields = new List<string>
            {
                nameof(SelectedPrivateCustomerName),
                nameof(SelectedPaymentPlan),
                nameof(SelectedLongTermSicknessAmount),
                nameof(SelectedAccidentDisabilityAmount),
                nameof(SelectedInsuranceAmount),

            };

            foreach (var field in validationFields)
            {
                string fieldError = this[field];
                if (!string.IsNullOrEmpty(fieldError))
                {
                    validationError += fieldError + "\n";
                }
            }

            if (!string.IsNullOrEmpty(validationError))
            {
                ValidationError = validationError;
                SetSnackbarMessage(validationError);
                return;
            }

            try
            {
                DateTime nextPaymentDate = CalculateNextPaymentDate(DateTime.Now, SelectedPaymentPlan.Type);
                string insuranceNumber = GenerateUniqueInsuranceNumber("PSV");

                PrivateInsurance privateInsurance = new PrivateInsurance
                {
                    Insured = new Insured
                    {
                        FirstName = SelectedPrivateCustomer.FirstName,
                        LastName = SelectedPrivateCustomer.LastName,
                        SSN = SelectedPrivateCustomer.SSN,
                    },

                    InsuranceNumber = insuranceNumber,
                    PrivateCustomerId = SelectedPrivateCustomer.Id,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1),
                    Note = Notes,
                    BaseAmount = SelectedInsuranceAmount,
                    Premium = CalculateMonthlyPremium(),
                    PaymentPlanId = SelectedPaymentPlan.Id,
                    InsuranceType = "Sjukförsäkring vuxen",
                    UserId = UserSession.GetInstance().User.Id,
                    InsuranceStatusId = 3,
                    AgentNumber = _employeeController.GetAgentNumberByEmployeeNumber(UserSession.GetInstance().User.EmployeeNumber),
                    PrivateInsuranceOptionalExtras = new List<PrivateInsuranceOptionalExtras>(),
                    NextPaymentDate = nextPaymentDate
                };

                if (SelectedLongTermSicknessAmount > 0)
                {
                    privateInsurance.PrivateInsuranceOptionalExtras.Add(new PrivateInsuranceOptionalExtras
                    {
                        Type = "Långvarig sjukskrivning",
                        BaseAmount = SelectedLongTermSicknessAmount,
                        PremiumSupplement = SelectedLongTermSicknessAmount * 0.005m
                    });
                }

                if (SelectedAccidentDisabilityAmount > 0)
                {
                    privateInsurance.PrivateInsuranceOptionalExtras.Add(new PrivateInsuranceOptionalExtras
                    {
                        Type = "Invaliditet vid olycksfall",
                        BaseAmount = SelectedAccidentDisabilityAmount,
                        PremiumSupplement = SelectedAccidentDisabilityAmount * 0.0003m
                    });
                }

                _privateInsuranceController.AddPrivateInsurance(privateInsurance);
                SetSnackbarMessage("Försäkring tillagd!");
            }
            catch (Exception ex)
            {
                SetSnackbarMessage($"Ett fel uppstod vid sparande av försäkringen: {ex.Message} {ex.InnerException}");
            }
        }

        private void SetSnackbarMessage(string message)
        {
            ValidationError = message;
            IsSnackbarActive = true;
        }


        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new RelayCommand(() =>
        {
            _mainViewModel.CurrentView = new HandleInsurancesUC
            {
                DataContext = new HandleInsurancesVM(_mainViewModel)
            };
        });

        public string this[string columnName]
        {
            get
            {
                if (!isSaveAttempted) return string.Empty;

                return columnName switch
                {
                    nameof(SelectedPrivateCustomerName) when SelectedPrivateCustomerName == null => "Vänligen välj en privatkund.",
                    nameof(SelectedPaymentPlan) when SelectedPaymentPlan == null => "Vänligen välj en betalningsplan.",
                    nameof(SelectedLongTermSicknessAmount) when IsLongSicknessChosen && SelectedLongTermSicknessAmount <= 0 => "Vänligen välj ett belopp för långvarig sjukskrivning.",
                    nameof(SelectedAccidentDisabilityAmount) when IsInvalidityChosen && SelectedAccidentDisabilityAmount <= 0 => "Vänligen ange ett giltigt belopp för invaliditet vid olycksfall.",
                    nameof(SelectedInsuranceAmount) when SelectedInsuranceAmount <= 0 => "Vänligen välj ett giltigt grundbelopp.",
                    _ => string.Empty
                };
            }
        }

        public string Error => string.Empty;

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(() => SavePrivateInsurance());
    }
}
