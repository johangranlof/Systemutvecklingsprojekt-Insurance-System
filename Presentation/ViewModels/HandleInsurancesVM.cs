using Business.Controllers;
using Business.Utilities;
using Entities;
using Presentation.Helpers;
using Presentation.Models;
using Presentation.Views;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Windows;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class HandleInsurancesVM : BaseVM
    {
        private readonly MainVM _mainViewModel;
        private readonly PrivateInsuranceController _privateInsuranceController;
        private readonly BusinessInsuranceController _businessInsuranceController;
        private readonly InvoiceGenerator _invoiceGenerator;

        public HandleInsurancesVM(MainVM mainViewModel)
        {
            _privateInsuranceController = new PrivateInsuranceController();
            _businessInsuranceController = new BusinessInsuranceController();
            _mainViewModel = mainViewModel;
            _invoiceGenerator = new InvoiceGenerator(_privateInsuranceController, _businessInsuranceController);

            var insuranceStatusUpdater = new InsuranceStatusUpdater();
            insuranceStatusUpdater.UpdateExpiredInsurances();

            LoadInsurances();
            CheckUser();
        }

        private void CheckUser()
        {
            var userPermissions = UserSession.GetInstance().User.Permission;
            CreateInsuranceOfferButton = userPermissions == "Säljare";
            CreateInvoiceFileButton = userPermissions == "Ekonomiassistent";
        }

        public bool CreateInsuranceOfferButton { get; private set; }
        public bool CreateInvoiceFileButton { get; private set; }

        private string _selectedFilter = "Alla";
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged(nameof(SelectedFilter));
                FilterInsurances();
            }
        }

        private ICommand _insuranceSearchCommand;
        public ICommand InsuranceSearchCommand => _insuranceSearchCommand ??= new RelayCommand(FilterInsurances);

        private string _insuranceSearchTerm;
        private System.Timers.Timer _searchTimer;
        private const int TypingDelay = 300; // Fördröjning i millisekunder, för att undvika att sökning körs för ofta

        public string InsuranceSearchTerm
        {
            get => _insuranceSearchTerm;
            set
            {
                _insuranceSearchTerm = value;
                OnPropertyChanged(nameof(InsuranceSearchTerm));
                _searchTimer?.Stop();
                _searchTimer = new System.Timers.Timer(TypingDelay) { AutoReset = false };
                _searchTimer.Elapsed += (s, e) => FilterInsurances();
                _searchTimer.Start();
            }
        }

        private List<InsuranceModel> _allInsurances;
        public List<InsuranceModel> AllInsurances
        {
            get => _allInsurances;
            set
            {
                _insurances = value;
                OnPropertyChanged(nameof(AllInsurances));
            }
        }

        private List<InsuranceModel> _insurances;
        public List<InsuranceModel> Insurances
        {
            get => _insurances;
            set
            {
                _insurances = value;
                OnPropertyChanged(nameof(Insurances));
            }
        }

        private InsuranceModel _selectedInsurance;
        public InsuranceModel SelectedInsurance
        {
            get => _selectedInsurance;
            set
            {
                _selectedInsurance = value;
                OnPropertyChanged(nameof(SelectedInsurance));
            }
        }

        private ICommand _createInvoiceFileCommand;
        public ICommand CreateInvoiceFileCommand => _createInvoiceFileCommand ??= new RelayCommand(() =>
        {
            _invoiceGenerator.GenerateInvoiceFile();

        });

        private ICommand _viewInsuranceCommand;
        public ICommand ViewInsuranceCommand => _viewInsuranceCommand ??= new RelayCommand(() =>
        {
            if (SelectedInsurance != null)
            {
                if (SelectedInsurance.IsPrivateCustomer)
                {
                    var privateInsurance = _privateInsuranceController.GetPrivateInsuranceById(SelectedInsurance.Id);
                    var viewPrivateInsuranceVM = new ViewPrivateInsuranceVM(_mainViewModel, privateInsurance);
                    var viewPrivateInsuranceUC = new ViewPrivateInsuranceUC { DataContext = viewPrivateInsuranceVM };
                    _mainViewModel.CurrentView = viewPrivateInsuranceUC;
                }
                else
                {
                    var businessInsurance = _businessInsuranceController.GetBusinessInsuranceById(SelectedInsurance.Id);
                    var viewBusinessInsuranceVM = new ViewBusinessInsuranceVM(_mainViewModel, businessInsurance);
                    var viewBusinessInsuranceUC = new ViewBusinessInsuranceUC { DataContext = viewBusinessInsuranceVM };
                    _mainViewModel.CurrentView = viewBusinessInsuranceUC;
                }
            }
            else
            {
                MessageBox.Show("Vänligen välj en försäkring innan du fortsätter.");
            }
        });

        private void LoadInsurances()
        {
            var privateInsurances = _privateInsuranceController.GetAllPrivateInsurances().Where(pi => pi.InsuranceStatusId != 2).Select(pi => new InsuranceModel
            {
                Id = pi.Id,
                InsuranceNumber = pi.InsuranceNumber,
                CustomerName = pi.PrivateCustomer.FullName,
                InsuranceType = pi.InsuranceType,
                IsPrivateCustomer = true,
                AgentNumber = pi.AgentNumber,
                Status = pi.InsuranceStatus.Status,
                Insured = pi.Insured.FullName,
                OrgSSN = pi.PrivateCustomer.SSN,
            });

            var businessInsurances = _businessInsuranceController.GetAllBusinessInsurances().Select(bi =>
            {
                string insuranceType;

                if (bi is LiabilityInsurance)
                    insuranceType = "Ansvarsförsäkring";
                else if (bi is RealEstateInsurance)
                    insuranceType = "Fastighetsförsäkring";
                else if (bi is VehicleInsurance)
                    insuranceType = "Fordonsförsäkring";
                else
                    insuranceType = "N/A";

                return new InsuranceModel
                {
                    Id = bi.Id,
                    InsuranceNumber = bi.InsuranceNumber,
                    CustomerName = bi.BusinessCustomer.CompanyName,
                    InsuranceType = insuranceType,
                    IsPrivateCustomer = false,
                    AgentNumber = bi.AgentNumber,
                    Status = bi.InsuranceStatus.Status,
                    Insured = bi.BusinessCustomer.CompanyName,
                    OrgSSN = bi.BusinessCustomer.OrganisationNumber,
                };
            });

            _allInsurances = privateInsurances.Concat(businessInsurances).ToList();
            Insurances = _allInsurances;
        }

        private void FilterBySearchTerm()
        {
            var filteredInsurances = _insurances;
            if (!string.IsNullOrWhiteSpace(InsuranceSearchTerm))
            {
                Insurances = filteredInsurances
                    .Where(i =>
                        i.InsuranceNumber.ToString().Contains(InsuranceSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        i.CustomerName.Contains(InsuranceSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        i.Insured.Contains(InsuranceSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        i.OrgSSN.Contains(InsuranceSearchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                Insurances = filteredInsurances;
            }
        }

        private void FilterInsurances()
        {
            var filteredInsurances = _allInsurances;
            _insurances = _allInsurances;
            FilterBySearchTerm();

            if (SelectedFilter == "Alla")
                filteredInsurances = _insurances.ToList();
            else if (SelectedFilter == "Företag")
                filteredInsurances = _insurances.Where(i => i.IsPrivateCustomer == false).ToList();
            else if (SelectedFilter == "Privat")
                filteredInsurances = _insurances.Where(i => i.IsPrivateCustomer == true).ToList();
            Insurances = filteredInsurances;
        }

        private ICommand _createInsuranceOfferCommand;
        public ICommand CreateInsuranceOfferCommand => _createInsuranceOfferCommand ??= new RelayCommand(() =>
        {
            _mainViewModel.IsOverlayVisible = true;

            var popupInsuranceWindow = new ChooseInsuranceTypeWindow();
            var createInsuranceOfferVM = new OfferInsuranceVM(_mainViewModel, popupInsuranceWindow);
            popupInsuranceWindow.DataContext = createInsuranceOfferVM;

            popupInsuranceWindow.Closed += (s, e) =>
            {
                _mainViewModel.IsOverlayVisible = false;
            };

            popupInsuranceWindow.ShowDialog();
        });
    }
}
