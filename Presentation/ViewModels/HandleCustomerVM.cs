using Business.Controllers;
using Business.Utilities;
using Presentation.Models;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Windows;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class HandleCustomerVM : BaseVM
    {
        private readonly MainVM _mainViewModel;
        private readonly EmployeeController _employeeController;
        private readonly PrivateCustomerController _privateCustomerController;
        private readonly BusinessCustomerController _businessCustomerController;

        public HandleCustomerVM(MainVM mainViewModel)
        {
            _privateCustomerController = new PrivateCustomerController();
            _businessCustomerController = new BusinessCustomerController();
            _employeeController = new EmployeeController();
            _mainViewModel = mainViewModel;
            Customers = new List<CustomerModel>();
            LoadCustomers();
            CheckUser();
        }

        private void CheckUser()
        {
            var userPermissions = UserSession.GetInstance().User.Permission;
            SellerFunctions = userPermissions == "Säljare" || userPermissions == "Försäljningsassistent";
        }

        public bool SellerFunctions { get; private set; }

        private string _selectedFilter = "Alla";
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged(nameof(SelectedFilter));
                FilterCustomers();
            }
        }

        private bool _showProspectsOnly;
        public bool ShowProspectsOnly
        {
            get => _showProspectsOnly;
            set
            {
                _showProspectsOnly = value;
                OnPropertyChanged(nameof(ShowProspectsOnly));
                FilterCustomers();
            }
        }

        private List<CustomerModel> _customers;
        public List<CustomerModel> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                OnPropertyChanged(nameof(Customers));
            }
        }
        private List<CustomerModel> _allCustomers;
        public List<CustomerModel> AllCustomers
        {
            get => _allCustomers;
            set
            {
                _customers = value;
                OnPropertyChanged(nameof(AllCustomers));
            }
        }


        private void FilterCustomers()
        {
            var filteredCustomers = _allCustomers;
            _customers = _allCustomers;
            FilterBySearchTerm();
            if (SelectedFilter == "Alla" && !ShowProspectsOnly)
                filteredCustomers = _customers.ToList();
            else if (SelectedFilter == "Företag" && !ShowProspectsOnly)
                filteredCustomers = _customers.Where(c => c.Type == "Företag").ToList();
            else if (SelectedFilter == "Privat" && !ShowProspectsOnly)
                filteredCustomers = _customers.Where(c => c.Type == "Privat").ToList();
            else if (SelectedFilter == "Alla" && ShowProspectsOnly)
                filteredCustomers = _customers.Where(c => c.IsProspect).ToList();
            else if (SelectedFilter == "Företag" && ShowProspectsOnly)
                filteredCustomers = _customers.Where(c => c.Type == "Företag" && c.IsProspect).ToList();
            else if (SelectedFilter == "Privat" && ShowProspectsOnly)
                filteredCustomers = _customers.Where(c => c.Type == "Privat" && c.IsProspect).ToList();
            Customers = filteredCustomers;
        }

        private void LoadCustomers()
        {
            var businessCustomer = _businessCustomerController.GetBusinessCustomers().Select(bc => new CustomerModel
            {
                Id = bc.Id,
                Type = "Företag",
                OrgSSN = bc.OrganisationNumber,
                Name = bc.CompanyName,
                IsPrivateCustomer = false,
                IsProspect = (bc.BusinessInsurances?.Count(i => i.InsuranceStatusId == 1) == 1)
            });
            var privateCustomer = _privateCustomerController.GetPrivateCustomers().Select(pc => new CustomerModel
            {
                Id = pc.Id,
                Type = "Privat",
                OrgSSN = pc.SSN,
                Name = pc.FirstName + " " + pc.LastName,
                IsPrivateCustomer = true,
                IsProspect = (pc.PrivateInsurances?.Count(i => i.InsuranceStatusId == 1) == 1)
            });

            _allCustomers = businessCustomer.Concat(privateCustomer).ToList();
            Customers = _allCustomers;
        }

        private ICommand _customerSearchCommand;
        public ICommand CustomerSearchCommand => _customerSearchCommand ??= new RelayCommand(FilterCustomers);

        private string _customerSearchTerm;
        private System.Timers.Timer _searchTimer;
        private const int TypingDelay = 300; // Fördröjning i millisekunder, för att undvika att sökning körs för ofta

        public string CustomerSearchTerm
        {
            get => _customerSearchTerm;
            set
            {
                _customerSearchTerm = value;
                OnPropertyChanged(nameof(CustomerSearchTerm));
                _searchTimer?.Stop();
                _searchTimer = new System.Timers.Timer(TypingDelay) { AutoReset = false };
                _searchTimer.Elapsed += (s, e) => FilterCustomers();
                _searchTimer.Start();
            }
        }

        private void FilterBySearchTerm()
        {
            var filteredCustomers = _customers;
            if (!string.IsNullOrWhiteSpace(CustomerSearchTerm))
            {
                Customers = filteredCustomers
                    .Where(i =>
                        i.Name.Contains(CustomerSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        i.OrgSSN.Contains(CustomerSearchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                Customers = filteredCustomers;
            }
        }

        private ICommand _addBusinessCustomerCommand;

        public ICommand AddBusinessCustomerCommand => _addBusinessCustomerCommand ??= new RelayCommand(() =>
        {
            var addBusinessCustomerVM = new AddBusinessCustomerVM(_mainViewModel);
            var addBusinessCustomerUC = new AddBusinessCustomerUC { DataContext = addBusinessCustomerVM };
            _mainViewModel.CurrentView = addBusinessCustomerUC;
        });

        private ICommand _addPrivateCustomerCommand;
        public ICommand AddPrivateCustomerCommand => _addPrivateCustomerCommand ??= new RelayCommand(() =>
        {
            var addPrivateCustomerVM = new AddPrivateCustomerVM(_mainViewModel);
            var addPrivateCustomerUC = new AddPrivateCustomerUC { DataContext = addPrivateCustomerVM };
            _mainViewModel.CurrentView = addPrivateCustomerUC;
        });

        private CustomerModel _selectedCustomer;
        public CustomerModel SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged(nameof(SelectedCustomer));
            }
        }

        private ICommand _viewCustomerCommand;
        public ICommand ViewCustomerCommand => _viewCustomerCommand ??= new RelayCommand(() =>
        {
            if (SelectedCustomer != null)
            {
                if (SelectedCustomer.IsPrivateCustomer)
                {
                    var privateCustomer = _privateCustomerController.GetPrivateCustomerByID(SelectedCustomer.Id);
                    var viewPrivateCustomerVM = new ViewPrivateCustomerVM(_mainViewModel, privateCustomer);
                    var viewPrivateCustomerUC = new ViewPrivateCustomerUC { DataContext = viewPrivateCustomerVM };
                    _mainViewModel.CurrentView = viewPrivateCustomerUC;
                }
                else
                {
                    var businessCustomer = _businessCustomerController.GetBusinessCustomerByID(SelectedCustomer.Id);
                    var viewBusinessCustomerVM = new ViewBusinessCustomerVM(_mainViewModel, businessCustomer);
                    var viewBusinessCustomerUC = new ViewBusinessCustomerUC { DataContext = viewBusinessCustomerVM };
                    _mainViewModel.CurrentView = viewBusinessCustomerUC;
                }
            }

            else
            {
                MessageBox.Show("Vänligen välj en kund innan du fortsätter.");
            }
        });
    }
}
