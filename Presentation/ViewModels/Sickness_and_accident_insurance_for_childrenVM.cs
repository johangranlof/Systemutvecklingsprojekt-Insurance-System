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
    public class Sickness_and_accident_insurance_for_childrenVM : BaseVM, IDataErrorInfo
    {
        private readonly MainVM _mainViewModel;
        private readonly PrivateInsuranceController _privateInsuranceController;
        private readonly PrivateCustomerController _privateCustomerController;
        private readonly EmployeeController _employeeController;
        private bool _isSnackbarActive;
        private bool isSaveAttempted = false;

        private ObservableCollection<int> _insuranceAmounts;
        public ObservableCollection<int> InsuranceAmounts
        {
            get { return _insuranceAmounts; }
            set
            {
                _insuranceAmounts = value;
                OnPropertyChanged(nameof(InsuranceAmounts));
            }
        }

        private ObservableCollection<int> _longTermSicknessAmounts;
        public ObservableCollection<int> LongTermSicknessAmounts
        {
            get { return _longTermSicknessAmounts; }
            set
            {
                _longTermSicknessAmounts = value;
                OnPropertyChanged(nameof(LongTermSicknessAmounts));
            }
        }

        private ObservableCollection<int> _accidentDisabilityAmounts;
        public ObservableCollection<int> AccidentDisabilityAmounts
        {
            get { return _accidentDisabilityAmounts; }
            set
            {
                _accidentDisabilityAmounts = value;
                OnPropertyChanged(nameof(AccidentDisabilityAmounts));
            }
        }

        public Sickness_and_accident_insurance_for_childrenVM(MainVM mainViewModel)
        {
            _privateInsuranceController = new PrivateInsuranceController();
            _mainViewModel = mainViewModel;
            _privateCustomerController = new PrivateCustomerController();
            _employeeController = new EmployeeController();
            _insuranceAmounts = new ObservableCollection<int> { 750000, 950000, 1150000, 1350000 };
            _longTermSicknessAmounts = new ObservableCollection<int> { 500, 1000, 1500, 2000, 2500, 3000, 3500, 4000 };
            _accidentDisabilityAmounts = new ObservableCollection<int> { 100000, 200000, 300000, 400000, 500000, 600000, 700000, 800000 };

            LoadPrivateCustomers();
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
            get { return _selectedAccidentDisabilityAmount; }
            set
            {
                _selectedAccidentDisabilityAmount = value;
                OnPropertyChanged(nameof(SelectedAccidentDisabilityAmount));
            }
        }

        private ObservableCollection<string> _privateCustomerName;
        public ObservableCollection<string> PrivateCustomerName
        {
            get { return _privateCustomerName; }
            set
            {
                _privateCustomerName = value;
                OnPropertyChanged(nameof(PrivateCustomerName));
            }
        }

        private string _insuredFirstName;
        public string InsuredFirstName
        {
            get { return _insuredFirstName; }
            set
            {
                _insuredFirstName = value;
                OnPropertyChanged(nameof(InsuredFirstName));
            }
        }

        private string _insuredLastName;
        public string InsuredLastName
        {
            get { return _insuredLastName; }
            set
            {
                _insuredLastName = value;
                OnPropertyChanged(nameof(InsuredLastName));
            }
        }

        private string _insuredSSN;
        public string InsuredSSN
        {
            get { return _insuredSSN; }
            set
            {
                _insuredSSN = value;
                OnPropertyChanged(nameof(InsuredSSN));
            }
        }

        private string _selectedPrivateCustomerName;
        public string SelectedPrivateCustomerName
        {
            get { return _selectedPrivateCustomerName; }
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

        private PrivateCustomer _selectedPrivateCustomer;
        public PrivateCustomer SelectedPrivateCustomer
        {
            get { return _selectedPrivateCustomer; }
            set
            {
                _selectedPrivateCustomer = value;
                OnPropertyChanged(nameof(SelectedPrivateCustomer));
            }
        }

        private int _selectedInsuranceAmount;
        public int SelectedInsuranceAmount
        {
            get { return _selectedInsuranceAmount; }
            set
            {
                _selectedInsuranceAmount = value;
                OnPropertyChanged(nameof(SelectedInsuranceAmount));
            }
        }

        private int _selectedLongTermSicknessAmount;
        public int SelectedLongTermSicknessAmount
        {
            get { return _selectedLongTermSicknessAmount; }
            set
            {
                _selectedLongTermSicknessAmount = value;
                OnPropertyChanged(nameof(SelectedLongTermSicknessAmount));
            }
        }

        private int _monthlyPayment;
        public int MonthlyPayment
        {
            get { return _monthlyPayment; }
            set
            {
                _monthlyPayment = value;
                OnPropertyChanged(nameof(MonthlyPayment));
            }
        }

        private ObservableCollection<PrivateCustomer> _allPrivateCustomers;
        public ObservableCollection<PrivateCustomer> AllPrivateCustomers
        {
            get { return _allPrivateCustomers; }
            set
            {
                _allPrivateCustomers = value;
                OnPropertyChanged(nameof(AllPrivateCustomers));
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
        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
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
                    case nameof(SelectedPrivateCustomerName):
                        if (SelectedPrivateCustomerName == null)
                        {
                            result = "Vänligen välj en giltig privatkund.";
                        }
                        break;

                    case nameof(SelectedPaymentPlan):
                        if (SelectedPaymentPlan == null)
                        {
                            result = "Vänligen välj en giltig betalningsplan.";
                        }
                        break;

                    case nameof(SelectedLongTermSicknessAmount):
                        if (IsLongSicknessChosen && SelectedLongTermSicknessAmount <= 0)
                        {
                            result = "Vänligen välj ett belopp för långvarig sjukskrivning.";
                        }
                        break;

                    case nameof(SelectedAccidentDisabilityAmount):
                        if (IsInvalidityChosen && SelectedAccidentDisabilityAmount <= 0)
                        {
                            result = "Vänligen ange ett giltigt belopp för invaliditet vid olycksfall.";
                        }
                        break;

                    case nameof(SelectedInsuranceAmount):
                        if (SelectedInsuranceAmount <= 0)
                        {
                            result = "Vänligen välj ett giltigt grundbelopp.";
                        }
                        break;

                    case nameof(InsuredSSN):
                        if (string.IsNullOrWhiteSpace(InsuredSSN))
                        {
                            result = "Försäkrades personnummer får inte vara tomt.";
                        }
                        else if (InsuredSSN.Length != 10 || !long.TryParse(InsuredSSN, out _))
                        {
                            result = "Försäkrades personnummer måste vara 10 siffror.";
                        }
                        break;

                    case nameof(InsuredFirstName):
                        if (string.IsNullOrWhiteSpace(InsuredFirstName))
                        {
                            result = "Förnamn får inte vara tomt.";
                        }
                        break;

                    case nameof(InsuredLastName):
                        if (string.IsNullOrWhiteSpace(InsuredLastName))
                        {
                            result = "Efternamn får inte vara tomt.";
                        }
                        break;
                }

                return result;
            }
        }

        public string Error => string.Empty;

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
            DateTime nextPaymentDate = startDate;

            switch (paymentPlanType)
            {
                case "Månad":
                    nextPaymentDate = startDate.AddMonths(1);
                    break;
                case "Kvartal":
                    nextPaymentDate = startDate.AddMonths(3);
                    break;
                case "Halvår":
                    nextPaymentDate = startDate.AddMonths(6);
                    break;
                case "Helår":
                    nextPaymentDate = startDate.AddYears(1);
                    break;
                default:
                    break;
            }

            return nextPaymentDate;
        }

        public int CalculateMonthlyPremium()
        {
            switch (SelectedInsuranceAmount)
            {
                case 750000:
                    MonthlyPayment = 375;
                    break;
                case 950000:
                    MonthlyPayment = 475;
                    break;
                case 1150000:
                    MonthlyPayment = 575;
                    break;
                case 1350000:
                    MonthlyPayment = 675;
                    break;
                default:
                    break;
            }
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
            OnPropertyChanged(nameof(InsuredSSN));
            OnPropertyChanged(nameof(InsuredFirstName));
            OnPropertyChanged(nameof(InsuredLastName));

            string validationError = string.Empty;
            var validationFields = new List<string>
            {
                nameof(SelectedPrivateCustomerName),
                nameof(SelectedPaymentPlan),
                nameof(SelectedLongTermSicknessAmount),
                nameof(SelectedAccidentDisabilityAmount),
                nameof(SelectedInsuranceAmount),
                nameof(InsuredSSN),
                nameof(InsuredSSN),
                nameof(InsuredLastName)
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
                return;
            }

            try
            {
                DateTime nextPaymentDate = CalculateNextPaymentDate(DateTime.Now, SelectedPaymentPlan.Type);
                string insuranceNumber = GenerateUniqueInsuranceNumber("PSB");

                PrivateInsurance privateInsurance = new PrivateInsurance
                {
                    Insured = new Insured
                    {
                        FirstName = InsuredFirstName,
                        LastName = InsuredLastName,
                        SSN = InsuredSSN
                    },
                    InsuranceNumber = insuranceNumber,
                    PrivateCustomerId = SelectedPrivateCustomer.Id,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1),
                    Note = Notes,
                    BaseAmount = SelectedInsuranceAmount,
                    Premium = CalculateMonthlyPremium(),
                    PaymentPlanId = SelectedPaymentPlan.Id,
                    InsuranceType = "Sjukförsäkring barn",
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
                SetSnackbarMessage($"Ett fel uppstod vid sparande av försäkringen: {ex.Message}");
            }
        }

        private void SetSnackbarMessage(string message)
        {
            ValidationError = message;
            IsSnackbarActive = true;
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ??= new RelayCommand(() => SavePrivateInsurance());

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

