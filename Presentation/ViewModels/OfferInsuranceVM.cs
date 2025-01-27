using Business.Controllers;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Windows;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class OfferInsuranceVM : BaseVM
    {

        private readonly MainVM _mainViewModel;
        private readonly PrivateCustomerController _privateCustomerController;
        private readonly Window _popupInsuranceWindow;


        public OfferInsuranceVM(MainVM mainViewModel, Window popupInsuranceWindow)
        {
            _privateCustomerController = new PrivateCustomerController();
            _mainViewModel = mainViewModel;
            _popupInsuranceWindow = popupInsuranceWindow;

        }

        private ICommand _offerlifeinsuranceCommand;
        public ICommand LifeInsuranceOfferCommand => _offerlifeinsuranceCommand ??= new RelayCommand(() =>
        {
            var LifeInsuranceOfferVM = new OfferLifeInsuranceVM(_mainViewModel);
            var LifeInsuranceOfferUC = new OfferLifeInsurance
            {
                DataContext = LifeInsuranceOfferVM
            };
            _popupInsuranceWindow.Close();

            _mainViewModel.CurrentView = LifeInsuranceOfferUC;
        });
        private ICommand _offersicknness_accidentCommand;
        public ICommand Offersicknness_accidentCommand => _offersicknness_accidentCommand ??= new RelayCommand(() =>
        {
            var sicknessInsuranceOfferVM = new Sickness_and_accident_insurance_for_childrenVM(_mainViewModel);
            var sicknessInsuranceOfferUC = new Sickness_and_accident_insurance_for_childrenUC
            {
                DataContext = sicknessInsuranceOfferVM
            };
            _popupInsuranceWindow.Close();
            _mainViewModel.CurrentView = sicknessInsuranceOfferUC;
        });


        private ICommand _offersicknness_accident_Adult_Command;
        public ICommand Offersicknness_accident_Adult_Command => _offersicknness_accident_Adult_Command ??= new RelayCommand(() =>
        {
            var sicknessInsuranceOfferVM = new Sickness_and_accident_insuranceVM(_mainViewModel);
            var sicknessInsuranceOfferUC = new Sickness_and_accident_insurance_UC
            {
                DataContext = sicknessInsuranceOfferVM
            };
            _popupInsuranceWindow.Close();
            _mainViewModel.CurrentView = sicknessInsuranceOfferUC;
        });


        private ICommand _offerResponsibilityInsuranceCommand;
        public ICommand OfferResponsibilityInsuranceCommand => _offerResponsibilityInsuranceCommand ??= new RelayCommand(() =>
        {
            var responsibbilityInsuranceVM = new ResponsibilityInsuranceVM(_mainViewModel);
            var responsibbilityInsuranceUC = new ResponsibilityInsuranceUC
            {
                DataContext = responsibbilityInsuranceVM
            };
            _popupInsuranceWindow.Close();
            _mainViewModel.CurrentView = responsibbilityInsuranceUC;
        });


        private ICommand _offerCarInsuranceCommand;
        public ICommand OfferCarInsuranceCommand => _offerCarInsuranceCommand ??= new RelayCommand(() =>
        {
            var offerCarInsuranceVM = new OfferCarInsuranceVM(_mainViewModel);
            var offerCarInsuranceUC = new OfferCarInsuranceUC
            {
                DataContext = offerCarInsuranceVM
            };
            _popupInsuranceWindow.Close();
            _mainViewModel.CurrentView = offerCarInsuranceUC;
        });

        private ICommand _offerRealEstateInsuranceCommand;
        public ICommand OfferRealEstateInsuranceCommand => _offerRealEstateInsuranceCommand ??= new RelayCommand(() =>
        {
            var offerRealEstateInsuranceVM = new OfferRealEstateInsuranceVM(_mainViewModel);
            var offerRealEstateInsuranceUC = new OfferRealEstateInsuranceUC
            {
                DataContext = offerRealEstateInsuranceVM
            };
            _popupInsuranceWindow.Close();
            _mainViewModel.CurrentView = offerRealEstateInsuranceUC;
        });

        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??= new RelayCommand(() =>
        {
            _popupInsuranceWindow.Close();
        });

    }
}
