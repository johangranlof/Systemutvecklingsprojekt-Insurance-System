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
    public class OfferRealEstateInsuranceVM : BaseVM, IDataErrorInfo
    {
        private readonly MainVM _mainViewModel;
        private readonly BusinessInsuranceController _businessInsuranceController;
        private readonly BusinessCustomerController _businessCustomerController;
        private readonly EmployeeController _employeeController;

        private bool _isSnackbarActive;

        public OfferRealEstateInsuranceVM(MainVM mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _businessCustomerController = new BusinessCustomerController();
            _businessInsuranceController = new BusinessInsuranceController();
            _employeeController = new EmployeeController();
            LoadBusinessCustomers();

            ValidationError = string.Empty;
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

        private string _streetAddress;
        public string StreetAddress
        {
            get { return _streetAddress; }
            set
            {
                _streetAddress = value;
                OnPropertyChanged(nameof(StreetAddress));
            }
        }

        private string _postalCode;
        public string PostalCode
        {
            get { return _postalCode; }
            set
            {
                _postalCode = value;
                OnPropertyChanged(nameof(PostalCode));
            }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                OnPropertyChanged(nameof(City));
            }
        }

        private double _propertyValue;
        public double PropertyValue
        {
            get { return _propertyValue; }
            set
            {
                _propertyValue = value;
                OnPropertyChanged(nameof(PropertyValue));
            }
        }

        private double _inventoryValue;
        public double InventoryValue
        {
            get { return _inventoryValue; }
            set
            {
                _inventoryValue = value;
                OnPropertyChanged(nameof(InventoryValue));
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

        private void LoadBusinessCustomers()
        {
            var businessCustomers = _businessCustomerController.GetBusinessCustomers();
            var businessCustomerNames = businessCustomers.Select(pc => $"{pc.CompanyName} {pc.OrganisationNumber}").ToList();
            var paymentPlans = _businessCustomerController.GetPaymentPlan().ToList();

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
                    case nameof(SelectedBusinessCustomerName):
                        if (SelectedBusinessCustomerName == null)
                            result = "Vänligen välj en giltig företagskund.";
                        break;
                    case nameof(SelectedPaymentPlan):
                        if (SelectedPaymentPlan == null)
                            result = "Vänligen välj en giltig betalningsplan.";
                        break;
                    case nameof(StreetAddress):
                        if (string.IsNullOrWhiteSpace(StreetAddress))
                            result = "Vänligen ange en giltig gatuadress.";
                        break;
                    case nameof(PostalCode):
                        if (string.IsNullOrWhiteSpace(PostalCode))
                            result = "Vänligen ange ett giltigt postnummer.";
                        break;
                    case nameof(City):
                        if (string.IsNullOrWhiteSpace(City))
                            result = "Vänligen ange en giltig ort.";
                        break;
                    case nameof(PropertyValue):
                        if (PropertyValue <= 0)
                            result = "Vänligen ange ett giltigt fastighetsvärde.";
                        break;
                    case nameof(InventoryValue):
                        if (InventoryValue <= 0)
                            result = "Vänligen ange ett giltigt inventarievärde.";
                        break;
                    case nameof(ContactFirstName):
                        if (string.IsNullOrWhiteSpace(ContactFirstName))
                            result = "Vänligen ange ett giltigt förnamn.";
                        break;
                    case nameof(ContactLastName):
                        if (string.IsNullOrWhiteSpace(ContactLastName))
                            result = "Vänligen ange ett giltigt efternamn.";
                        break;
                    case nameof(ContactPhoneNumber):
                        if (string.IsNullOrWhiteSpace(ContactPhoneNumber))
                            result = "Vänligen ange ett giltigt telefonnummer.";
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

        private void Save()
        {
            isSaveAttempted = true;

            string validationError = string.Empty;
            var validationFields = new List<string>
            {
                nameof(SelectedBusinessCustomerName),
                nameof(SelectedPaymentPlan),
                nameof(StreetAddress),
                nameof(PostalCode),
                nameof(City),
                nameof(PropertyValue),
                nameof(InventoryValue),
                nameof(ContactFirstName),
                nameof(ContactLastName),
                nameof(ContactPhoneNumber)
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

            DateTime nextPaymentDate = CalculateNextPaymentDate(DateTime.Now, SelectedPaymentPlan.Type);
            string insuranceNumber = GenerateUniqueInsuranceNumber("FGF");

            var realEstateInsurance = new RealEstateInsurance
            {
                ContactFirstName = ContactFirstName,
                ContactLastName = ContactLastName,
                ContactPhoneNumber = ContactPhoneNumber,
                ZipCode = PostalCode,
                StreetAddress = StreetAddress,
                City = City,
                PropertyValue = (int)PropertyValue,
                InventoryValue = (decimal)InventoryValue,
                InventoryPrice = ((decimal)(InventoryValue * 0.002)),
                RealEstatePremium = (int)(PropertyValue * 0.002),
                Premium = (int)(InventoryValue * 0.002) + (int)(PropertyValue * 0.002),
                InsuranceNumber = insuranceNumber,
                BusinessCustomerId = SelectedBusinessCustomer.Id,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddYears(1),
                Note = Note,
                PaymentPlanId = SelectedPaymentPlan.Id,
                UserId = UserSession.GetInstance().User.Id,
                InsuranceStatusId = 3,
                AgentNumber = _employeeController.GetAgentNumberByEmployeeNumber(UserSession.GetInstance().User.EmployeeNumber),
                NextPaymentDate = nextPaymentDate
            };

            _businessInsuranceController.AddRealEstateInsurance(realEstateInsurance);
            ValidationError = "Försäkring tillagd!";
            IsSnackbarActive = true;
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
