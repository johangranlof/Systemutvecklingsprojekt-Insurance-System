using Business.Controllers;
using Business.Utilities;
using Entities;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class OfferLifeInsuranceVM : BaseVM, IDataErrorInfo
    {
        private readonly MainVM _mainViewModel;
        private readonly PrivateInsuranceController _privateInsuranceController;
        private readonly PrivateCustomerController _privateCustomerController;
        private readonly EmployeeController _employeeController;
        private bool _isSnackbarActive;

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

        public OfferLifeInsuranceVM(MainVM mainViewModel)
        {
            _privateInsuranceController = new PrivateInsuranceController();
            _mainViewModel = mainViewModel;
            _privateCustomerController = new PrivateCustomerController();
            _employeeController = new EmployeeController();
            _insuranceAmounts = new ObservableCollection<int> { 350000, 450000, 550000 };

            ValidationError = string.Empty;

            LoadPrivateCustomers();
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

        public int MonthlyPayment;

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
            string normalizedPaymentPlanType = paymentPlanType;

            switch (normalizedPaymentPlanType)
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

        private bool isSaveAttempted = false;

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
                            result = "Vänligen välj en giltig privat kund.";
                        break;
                    case nameof(SelectedPaymentPlan):
                        if (SelectedPaymentPlan == null)
                            result = "Vänligen välj en giltig betalningsplan.";
                        break;
                    case nameof(SelectedInsuranceAmount):
                        if (SelectedInsuranceAmount <= 0)
                            result = "Vänligen välj ett giltigt grundbelopp.";
                        break;
                    default:
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

        public void SavePrivateInsurance()
        {
            isSaveAttempted = true;

            string validationError = string.Empty;
            var validationFields = new List<string>
            {
                nameof(SelectedPrivateCustomerName),
                nameof(SelectedPaymentPlan),
                nameof(SelectedInsuranceAmount)
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

            var userSession = UserSession.GetInstance();
            var currentUser = userSession.User;

            if (SelectedInsuranceAmount == 350000)
            {
                MonthlyPayment = 175;
            }
            else if (SelectedInsuranceAmount == 450000)
            {
                MonthlyPayment = 225;
            }
            else if (SelectedInsuranceAmount == 550000)
            {
                MonthlyPayment = 275;
            }

            int agentNumber = _employeeController.GetAgentNumberByEmployeeNumber(currentUser.EmployeeNumber);
            if (agentNumber == null)
            {
                MessageBox.Show("Agent number could not be found for the current user.");
                return;
            }

            Insured insured = _privateInsuranceController.GetPrivateInsuredBySSN(SelectedPrivateCustomer.SSN);
            if (insured == null)
            {
                insured = new Insured
                {
                    SSN = SelectedPrivateCustomer.SSN,
                    FirstName = SelectedPrivateCustomer.FirstName,
                    LastName = SelectedPrivateCustomer.LastName,
                };
            }

            DateTime nextPaymentDate = CalculateNextPaymentDate(DateTime.Now, SelectedPaymentPlan.Type);
            string insuranceNumber = GenerateUniqueInsuranceNumber("PLF"); // Privat livförsäkring 
            var privateInsurance = new PrivateInsurance
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(1),
                Note = Notes,
                BaseAmount = SelectedInsuranceAmount,
                InsuranceNumber = insuranceNumber,
                Premium = MonthlyPayment,
                Anonymized = false,
                PrivateCustomerId = SelectedPrivateCustomer.Id,
                InsuranceType = "Livförsäkring vuxen",
                PaymentPlanId = SelectedPaymentPlan.Id,
                UserId = currentUser.Id,
                InsuranceStatusId = 3,
                Insured = insured,
                AgentNumber = agentNumber,
                NextPaymentDate = nextPaymentDate
            };

            _privateInsuranceController.AddPrivateInsurance(privateInsurance);
            ValidationError = "Försäkring tillagd!";
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
