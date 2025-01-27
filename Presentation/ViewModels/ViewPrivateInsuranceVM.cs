using Business.Controllers;
using Business.Utilities;
using Entities;
using Presentation.Views;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Windows;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class ViewPrivateInsuranceVM : BaseVM
    {
        private readonly MainVM _mainViewModel;
        public PrivateInsurance PrivateInsurance { get; set; }
        private readonly PrivateInsuranceController _privateInsuranceController;
        public bool _isSnackbarActive;

        public ViewPrivateInsuranceVM(MainVM mainViewModel, PrivateInsurance privateInsurance)
        {
            _mainViewModel = mainViewModel;
            PrivateInsurance = privateInsurance;
            _privateInsuranceController = new PrivateInsuranceController();
            ValidationError = string.Empty;
            if (PrivateInsurance.PrivateInsuranceOptionalExtras == null)
            {
                PrivateInsurance.PrivateInsuranceOptionalExtras = _privateInsuranceController.GetPrivateOptionalInsurances(PrivateInsurance.Id);
            }
            UpdateButtonVisibility();
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

        private InsuranceStatus _insuranceStatus;

        public InsuranceStatus InsuranceStatus
        {
            get => _insuranceStatus;
            set
            {
                if (_insuranceStatus != value)
                {
                    _insuranceStatus = value;
                    OnPropertyChanged(nameof(InsuranceStatus));
                }
            }
        }
        private void UpdateButtonVisibility()
        {
            IsButtonVisible = CanRemoveInsurance() && IsUserAuthorizedToRemove();
        }
        private void RemoveInsurance()
        {
            var result = MessageBox.Show(
                "Är du säker på att du vill ta bort denna privata försäkring?",
                "Bekräfta borttagning",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _privateInsuranceController.RemovePrivateInsurance(PrivateInsurance);
                    UpdateButtonVisibility();
                    OnPropertyChanged(nameof(PrivateInsurance));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ett fel inträffade när den privata försäkringen skulle tas bort: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private bool CanRemoveInsurance()
        {
            return PrivateInsurance.InsuranceStatusId == 3;
        }

        private bool IsUserAuthorizedToRemove()
        {
            var userPermission = UserSession.GetInstance().User.Permission;
            return userPermission == "Säljare" || userPermission == "Försäljningsassistent";
        }

        private ICommand _removeInsuranceCommand;
        public ICommand RemoveInsuranceCommand => _removeInsuranceCommand ??= new RelayCommand(() =>
        {
            RemoveInsurance();
            BackCommand.Execute(null);
        }, () => CanRemoveInsurance());

        private ICommand _activateInsuranceCommand;
        public ICommand ActivateInsuranceCommand => _activateInsuranceCommand ??= new RelayCommand(() =>
        {
            _mainViewModel.IsOverlayVisible = true;
            var selectStartDatePopup = new SelectStartDatePopup();
            var selectStartDateVM = new SelectStartDateVM(PrivateInsurance, selectStartDatePopup, _mainViewModel, this);
            selectStartDatePopup.DataContext = selectStartDateVM;
            selectStartDatePopup.Closed += (s, e) =>
            {
                _mainViewModel.IsOverlayVisible = false;
            };
            selectStartDatePopup.ShowDialog();
            OnPropertyChanged(nameof(PrivateInsurance));
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
