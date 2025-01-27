using Business.Controllers;
using Business.Utilities;
using Entities;
using Presentation.Views;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class ViewBusinessInsuranceVM : BaseVM
    {
        private readonly MainVM _mainViewModel;
        public BusinessInsurance BusinessInsurance { get; set; }
        private readonly BusinessInsuranceController _businessInsuranceController;
        public bool _isSnackbarActive;
        private ObservableCollection<VehicleInsurance> _vehicleInsuranceList;
        private ObservableCollection<RealEstateInsurance> _realEstateInsuranceList;

        public ViewBusinessInsuranceVM(MainVM mainViewModel, BusinessInsurance businessInsurance)
        {
            _mainViewModel = mainViewModel;
            BusinessInsurance = businessInsurance;
            _businessInsuranceController = new BusinessInsuranceController();
            UpdateButtonVisibility();
            ValidationError = string.Empty;
            _insuranceType = GetInsuranceType();
            LoadVehicleInsurances();
            LoadRealEstateInsurances();
            LoadLiabilityInsurances();
        }

        public ObservableCollection<VehicleInsurance> VehicleInsuranceList
        {
            get => _vehicleInsuranceList;
            set
            {
                _vehicleInsuranceList = value;
                OnPropertyChanged(nameof(VehicleInsuranceList));
                if (VehicleInsuranceCollectionView != null)
                {
                    VehicleInsuranceCollectionView.Refresh();
                }
            }
        }

        public ObservableCollection<RealEstateInsurance> RealEstateInsuranceList
        {
            get => _realEstateInsuranceList;
            set
            {
                _realEstateInsuranceList = value;
                OnPropertyChanged(nameof(RealEstateInsuranceList));
                if (RealEstateInsuranceCollectionView != null)
                {
                    RealEstateInsuranceCollectionView.Refresh();
                }
            }
        }

        private ICollectionView _vehicleInsuranceCollectionView;
        public ICollectionView VehicleInsuranceCollectionView
        {
            get => _vehicleInsuranceCollectionView;
            private set
            {
                if (_vehicleInsuranceCollectionView != value)
                {
                    _vehicleInsuranceCollectionView = value;
                    OnPropertyChanged(nameof(VehicleInsuranceCollectionView));
                }
            }
        }


        private ICollectionView _realEstateInsuranceCollectionView;
        public ICollectionView RealEstateInsuranceCollectionView
        {
            get => _realEstateInsuranceCollectionView;
            private set
            {
                if (_realEstateInsuranceCollectionView != value)
                {
                    _realEstateInsuranceCollectionView = value;
                    OnPropertyChanged(nameof(RealEstateInsuranceCollectionView));
                }
            }
        }

        private void LoadVehicleInsurances()
        {
            var vehicleInsurances = _businessInsuranceController.GetVehicleInsurancesByBusinessCustomerId(BusinessInsurance.BusinessCustomer.Id);


            VehicleInsuranceList = new ObservableCollection<VehicleInsurance>(vehicleInsurances);

            VehicleInsuranceCollectionView = CollectionViewSource.GetDefaultView(VehicleInsuranceList);
            VehicleInsuranceCollectionView.Filter = FilterVehicleInsurances;
        }


        private void LoadLiabilityInsurances()
        {
            var liabilityInsurances = _businessInsuranceController.GetLiabilityInsurancesByBusinessCustomerId(BusinessInsurance.BusinessCustomer.Id);

            LiabilityInsuranceList = new ObservableCollection<LiabilityInsurance>(liabilityInsurances);

            LiabilityInsuranceCollectionView = CollectionViewSource.GetDefaultView(LiabilityInsuranceList);
            LiabilityInsuranceCollectionView.Filter = FilterLiabilityInsurances;
        }

        private bool FilterLiabilityInsurances(object item)
        {
            if (item is LiabilityInsurance liabilityInsurance)
            {
                return liabilityInsurance.InsuranceNumber == BusinessInsurance.InsuranceNumber;
            }
            return false;
        }

        private void LoadRealEstateInsurances()
        {
            var realEstateInsurances = _businessInsuranceController.GetRealEstateInsurancesByBusinessCustomerId(BusinessInsurance.BusinessCustomer.Id);

            RealEstateInsuranceList = new ObservableCollection<RealEstateInsurance>(realEstateInsurances);

            RealEstateInsuranceCollectionView = CollectionViewSource.GetDefaultView(RealEstateInsuranceList);
            RealEstateInsuranceCollectionView.Filter = FilterRealEstateInsurances;
        }

        private bool FilterRealEstateInsurances(object item)
        {
            if (item is RealEstateInsurance realEstateInsurance)
            {
                return realEstateInsurance.InsuranceNumber == BusinessInsurance.InsuranceNumber;
            }
            return false;
        }

        private bool FilterVehicleInsurances(object item)
        {
            if (item is VehicleInsurance vehicleInsurance)
            {
                return vehicleInsurance.InsuranceNumber == BusinessInsurance.InsuranceNumber;
            }
            return false;
        }

        public ObservableCollection<string> CoverageTypeList
        {
            get
            {
                return new ObservableCollection<string>(
                    VehicleInsuranceList.Select(v => CoverageToString(v.Extent))
                );
            }
        }

        public ObservableCollection<string> ZoneList
        {
            get
            {
                return new ObservableCollection<string>(
                    VehicleInsuranceList.Select(v => ZoneToString(v.ZoneId))
                );
            }
        }

        public ObservableCollection<string> RealEstatePropertyValueList
        {
            get
            {
                return new ObservableCollection<string>(
                    RealEstateInsuranceList.Select(r => $"{r.PropertyValue:N2} kr")
                );
            }
        }

        public ObservableCollection<string> RealEstateInventoryValueList
        {
            get
            {
                return new ObservableCollection<string>(
                    RealEstateInsuranceList.Select(r => $"{r.InventoryValue:N2} kr")
                );
            }
        }

        public ObservableCollection<string> RealEstatePropertyPremiumList
        {
            get
            {
                return new ObservableCollection<string>(
                    RealEstateInsuranceList.Select(r => $"{r.RealEstatePremium:N2} kr")
                );
            }
        }

        public ObservableCollection<string> RealEstateInventoryPremiumList
        {
            get
            {
                return new ObservableCollection<string>(
                    RealEstateInsuranceList.Select(r => $"{r.InventoryPrice:N2} kr")
                );
            }
        }

        private string CoverageToString(int coverageExtent)
        {
            switch (coverageExtent)
            {
                case 1:
                    return "1: Trafikförsäkring";
                case 2:
                    return "2: Halvförsäkring";
                case 3:
                    return "3: Helförsäkring";
                default:
                    return "N/A";
            }
        }

        private string ZoneToString(int zoneId)
        {
            switch (zoneId)
            {
                case 1:
                    return "1: Hög riskzon";
                case 2:
                    return "2: Mellan riskzon";
                case 3:
                    return "3: Låg riskzon";
                case 4:
                    return "4: Mycket låg riskzon";
                default:
                    return "Okänd zon";
            }
        }

        public ObservableCollection<string> LiabilityInsuranceExtentList
        {
            get
            {
                return new ObservableCollection<string>(
                    LiabilityInsuranceList.Select(l => $"{l.Extent:N0} kr")
                );
            }
        }

        public ObservableCollection<string> LiabilityInsuranceDeductibleList
        {
            get
            {
                return new ObservableCollection<string>(
                    LiabilityInsuranceList.Select(l => $"{l.Deductible:N2} kr")
                );
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

        private bool _isButtonVisible;
        public bool IsButtonVisible
        {
            get => _isButtonVisible;
            set
            {
                if (_isButtonVisible != value)
                {
                    _isButtonVisible = value;
                    OnPropertyChanged(nameof(IsButtonVisible));
                }
            }
        }

        private bool _isVehicleInsuranceVisible;
        public bool IsVehicleInsuranceVisible
        {
            get => _isVehicleInsuranceVisible;
            set
            {
                _isVehicleInsuranceVisible = value;
                OnPropertyChanged(nameof(IsVehicleInsuranceVisible));
            }
        }

        private bool _isRealEstateInsuranceVisible;
        public bool IsRealEstateInsuranceVisible
        {
            get => _isRealEstateInsuranceVisible;
            set
            {
                _isRealEstateInsuranceVisible = value;
                OnPropertyChanged(nameof(IsRealEstateInsuranceVisible));
            }
        }

        private bool _isLiabilityInsuranceVisible;
        public bool IsLiabilityInsuranceVisible
        {
            get => _isLiabilityInsuranceVisible;
            set
            {
                _isLiabilityInsuranceVisible = value;
                OnPropertyChanged(nameof(IsLiabilityInsuranceVisible));
            }
        }
        private string _insuranceType;
        public string InsuranceType
        {
            get => _insuranceType;
            set
            {
                _insuranceType = value;
                OnPropertyChanged(nameof(InsuranceType));
            }
        }
        private ObservableCollection<LiabilityInsurance> _liabilityInsuranceList;

        public ObservableCollection<LiabilityInsurance> LiabilityInsuranceList
        {
            get => _liabilityInsuranceList;
            set
            {
                _liabilityInsuranceList = value;
                OnPropertyChanged(nameof(LiabilityInsuranceList));
                if (LiabilityInsuranceCollectionView != null)
                {
                    LiabilityInsuranceCollectionView.Refresh();
                }
            }
        }

        private ICollectionView _liabilityInsuranceCollectionView;
        public ICollectionView LiabilityInsuranceCollectionView
        {
            get => _liabilityInsuranceCollectionView;
            private set
            {
                if (_liabilityInsuranceCollectionView != value)
                {
                    _liabilityInsuranceCollectionView = value;
                    OnPropertyChanged(nameof(LiabilityInsuranceCollectionView));
                }
            }
        }

        private void UpdateButtonVisibility()
        {
            IsButtonVisible = CanRemoveInsurance() && IsUserAuthorizedToRemove();
        }

        private void RemoveInsurance()
        {
            var existingInsurance = _businessInsuranceController.GetBusinessInsuranceById(BusinessInsurance.Id);

            if (existingInsurance != null)
            {
                _businessInsuranceController.RemoveBusinessInsurance(existingInsurance);
                UpdateButtonVisibility();
            }
        }

        private bool CanRemoveInsurance()
        {
            return BusinessInsurance.InsuranceStatusId == 3;
        }

        private bool IsUserAuthorizedToRemove()
        {
            var userPermission = UserSession.GetInstance().User.Permission;
            return userPermission == "Säljare" || userPermission == "Försäljningsassistent";
        }

        private string GetInsuranceType()
        {
            var businessInsurance = _businessInsuranceController.GetBusinessInsuranceById(BusinessInsurance.Id);

            string insuranceType;

            if (businessInsurance is LiabilityInsurance)
                insuranceType = "Ansvarsförsäkring";
            else if (businessInsurance is RealEstateInsurance)
                insuranceType = "Fastighet och inventarieförsäkring";
            else if (businessInsurance is VehicleInsurance)
                insuranceType = "Fordonsförsäkring personbil";
            else
                insuranceType = "N/A";

            return insuranceType;
        }

        private ICommand _removeInsuranceCommand;
        public ICommand RemoveInsuranceCommand => _removeInsuranceCommand ??= new RelayCommand(() =>
        {
            var result = MessageBox.Show(
                "Är du säker på att du vill ta bort denna försäkring?",
                "Bekräfta borttagning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                RemoveInsurance();
                BackCommand.Execute(null);
            }
        }, () => CanRemoveInsurance());

        private ICommand _activateInsuranceCommand;
        public ICommand ActivateInsuranceCommand => _activateInsuranceCommand ??= new RelayCommand(() =>
        {
            _mainViewModel.IsOverlayVisible = true;
            var selectStartDatePopup = new SelectStartDatePopup();
            var selectStartDateVM = new SelectStartDateVM(BusinessInsurance, selectStartDatePopup, _mainViewModel, this);
            selectStartDatePopup.DataContext = selectStartDateVM;
            selectStartDatePopup.Closed += (s, e) =>
            {
                _mainViewModel.IsOverlayVisible = false;
            };

            selectStartDatePopup.ShowDialog();
            OnPropertyChanged(nameof(BusinessInsurance));
        });

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
