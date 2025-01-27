using Business.Controllers;
using Entities;
using PresentationLayer.Commands;
using System.Windows;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class SelectStartDateVM : BaseVM
    {
        private readonly MainVM _mainViewModel;
        private readonly Window _popupWindow;
        public BusinessInsurance BusinessInsurance { get; set; }
        public PrivateInsurance PrivateInsurance { get; set; }
        public ViewPrivateInsuranceVM _viewPrivateInsuranceVM;
        public ViewBusinessInsuranceVM _viewBusinessInsuranceVM;


        private DateTime _pickedDate;
        public DateTime PickedDate
        {
            get => _pickedDate;
            set
            {
                _pickedDate = value;
                OnPropertyChanged(nameof(PickedDate));
            }
        }
        public SelectStartDateVM(BusinessInsurance businessInsurance, Window popupWindow, MainVM mainViewModel, ViewBusinessInsuranceVM viewBusinessInsurance)
        {
            PickedDate = DateTime.Now;
            BusinessInsurance = businessInsurance;
            _popupWindow = popupWindow;
            _mainViewModel = mainViewModel;
            _viewBusinessInsuranceVM = viewBusinessInsurance;

        }

        public SelectStartDateVM(PrivateInsurance privateInsurance, Window popupWindow, MainVM mainViewModel, ViewPrivateInsuranceVM viewPrivateInsurance)
        {
            PickedDate = DateTime.Now;
            PrivateInsurance = privateInsurance;
            _popupWindow = popupWindow;
            _mainViewModel = mainViewModel;
            _viewPrivateInsuranceVM = viewPrivateInsurance;
        }


        private ICommand _confirmCommand;
        public ICommand ConfirmCommand => _confirmCommand ??= new RelayCommand(() =>
        {
            DateTime endDate = PickedDate.AddYears(1);

            if (PrivateInsurance != null)
            {
                PrivateInsurance.StartDate = PickedDate;
                PrivateInsurance.EndDate = endDate;
                PrivateInsurance.InsuranceStatusId = 1;

                if (PrivateInsurance.InsuranceStatus == null)
                {
                    PrivateInsurance.InsuranceStatus = new InsuranceStatus();
                }

                PrivateInsurance.InsuranceStatus.Status = "Aktiv";
                var privateInsuranceController = new PrivateInsuranceController();
                privateInsuranceController.UpdatePrivateInsurance(PrivateInsurance);

                _popupWindow.Close();

                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _viewPrivateInsuranceVM.ValidationError = "Försäkring aktiverad!";
                    _viewPrivateInsuranceVM.IsSnackbarActive = true;
                }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            }
            else if (BusinessInsurance != null)
            {
                BusinessInsurance.StartDate = PickedDate;
                BusinessInsurance.EndDate = endDate;
                BusinessInsurance.InsuranceStatusId = 1;

                if (BusinessInsurance.InsuranceStatus == null)
                {
                    BusinessInsurance.InsuranceStatus = new InsuranceStatus();
                }

                BusinessInsurance.InsuranceStatus.Status = "Aktiv";
                var businessInsuranceController = new BusinessInsuranceController();
                businessInsuranceController.UpdateBusinessInsurance(BusinessInsurance);
                _popupWindow.Close();

                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _viewBusinessInsuranceVM.ValidationError = "Försäkring aktiverad!";
                    _viewBusinessInsuranceVM.IsSnackbarActive = true;
                }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            }
        });


        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(() =>
        {
            _popupWindow.Close();
        });


    }
}
