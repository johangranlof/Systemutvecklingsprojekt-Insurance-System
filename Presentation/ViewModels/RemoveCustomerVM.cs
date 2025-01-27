using Business.Controllers;
using Entities;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Windows;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class RemoveCustomerVM : BaseVM
    {
        private readonly MainVM _mainViewModel;
        private readonly Window _popupWindow;
        private readonly BusinessCustomerController _businessCustomerController;
        private readonly PrivateCustomerController _privateCustomerController;

        private readonly BusinessCustomer _businessCustomer;
        private readonly PrivateCustomer _privateCustomer;

        public RemoveCustomerVM(BusinessCustomer businessCustomer, Window popupWindow, MainVM mainViewModel, BusinessCustomerController businessCustomerController)
        {
            _businessCustomer = businessCustomer;
            _popupWindow = popupWindow;
            _mainViewModel = mainViewModel;
            _businessCustomerController = businessCustomerController;
        }

        public RemoveCustomerVM(PrivateCustomer privateCustomer, Window popupWindow, MainVM mainViewModel, PrivateCustomerController privateCustomerController)
        {
            _privateCustomer = privateCustomer;
            _popupWindow = popupWindow;
            _mainViewModel = mainViewModel;
            _privateCustomerController = privateCustomerController;
        }

        public string Name => _businessCustomer != null
         ? _businessCustomer.CompanyName
         : $"{_privateCustomer.FirstName} {_privateCustomer.LastName}";

        public ICommand ConfirmCommand => new RelayCommand(() =>
        {
            RemoveCustomer();
            _popupWindow.Close();
        });

        public ICommand CancelCommand => new RelayCommand(() =>
        {
            _popupWindow.Close();
        });

        private void RemoveCustomer()
        {
            if (_businessCustomer != null)
            {
                if (_businessCustomerController.HasActiveInsurance(_businessCustomer))
                {
                    MessageBox.Show("Kunden kan inte tas bort eftersom det finns aktiva försäkringar.", "Borttagning Blockerad", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    _businessCustomerController.DeleteBusinessCustomer(_businessCustomer);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ett fel uppstod vid borttagning av företagskunden: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (_privateCustomer != null)
            {
                if (_privateCustomerController.HasActiveInsurance(_privateCustomer))
                {
                    MessageBox.Show("Kunden kan inte tas bort eftersom det finns aktiva försäkringar.", "Borttagning Blockerad", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    _privateCustomerController.DeletePrivateCustomer(_privateCustomer);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ett fel uppstod vid borttagning av privatkunden: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            _mainViewModel.CurrentView = new HandleCustomerUC
            {
                DataContext = new HandleCustomerVM(_mainViewModel)
            };
        }
    }
}
