using Business.Controllers;
using Business.Utilities;
using Presentation.Helpers;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Windows.Controls;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class MainVM : BaseVM
    {
        private readonly MenuService _menuService = new MenuService();
        private readonly EmployeeController _employeeController = new EmployeeController();

        private bool _isOverlayVisible;
        public bool IsOverlayVisible
        {
            get => _isOverlayVisible;
            set
            {
                _isOverlayVisible = value;
                OnPropertyChanged(nameof(IsOverlayVisible));
            }
        }

        public MainVM()
        {
            SetMenuItemsBasedOnRole();
            SetInitialView();
        }

        private void SetInitialView()
        {
            var firstMenuItem = MenuItems?.FirstOrDefault();

            if (firstMenuItem != null)
            {
                switch (firstMenuItem.Name)
                {
                    case "Anställd":
                        HandleEmployeeCommand.Execute(null);
                        break;
                    case "Provision":
                        HandleComissionCommand.Execute(null);
                        break;
                    case "Försäkring":
                        HandleInsurancesCommand.Execute(null);
                        break;
                    case "Kund":
                        HandleCustomerCommand.Execute(null);
                        break;
                    case "Försäljningsstatistik":
                        AnalyzeSalesStatisticsCommand.Execute(null);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetMenuItemsBasedOnRole()
        {
            var userPermission = UserSession.GetInstance().User.Permission;
            MenuItems = _menuService.GetMenuItemsForRole(userPermission,
                HandleEmployeeCommand,
                HandleComissionCommand,
                HandleInsurancesCommand,
                HandleCustomerCommand,
                AnalyzeSalesStatisticsCommand
                );

            var employee = _employeeController.GetEmployeeByEmployeeNumber(UserSession.GetInstance().User.EmployeeNumber);
            EmployeeName = employee.FirstName + " " + employee.LastName;
            _employeeInitials = employee.FirstName.Substring(0, 1) + employee.LastName.Substring(0, 1);

            UserPermission = userPermission;
        }

        private string _userPermission;
        public string UserPermission
        {
            get => _userPermission;
            set
            {
                _userPermission = value;
                OnPropertyChanged(nameof(UserPermission));
            }
        }

        private string _employeeInitials;
        public string EmployeeInitials
        {
            get => _employeeInitials;
            set
            {
                _employeeInitials = value;
                OnPropertyChanged(nameof(EmployeeInitials));
            }
        }

        private string _employeeName;
        public string EmployeeName
        {
            get => _employeeName;
            set
            {
                _employeeName = value;
                OnPropertyChanged(nameof(EmployeeName));
            }
        }

        private UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private List<MenuObject> _menuItems;
        public List<MenuObject> MenuItems
        {
            get => _menuItems;
            set
            {
                _menuItems = value;
                OnPropertyChanged(nameof(MenuItems));
            }
        }

        private ICommand _handleEmployeeCommand = null!;
        public ICommand HandleEmployeeCommand => _handleEmployeeCommand ??= new RelayCommand(() =>
        {
            foreach (var item in MenuItems)
            {
                item.IsChecked = false;

            }
            var employeeMenuItem = MenuItems.FirstOrDefault(item => item.Name == "Anställd");

            if (employeeMenuItem != null)
            {
                employeeMenuItem.IsChecked = true;
            }

            var handleEmployeeVM = new HandleEmployeeVM(this);
            var handleEmployeeUC = new HandleEmployeeUC
            {
                DataContext = handleEmployeeVM
            };
            CurrentView = handleEmployeeUC;
        });

        private ICommand _handleComissionCommand = null!;
        public ICommand HandleComissionCommand => _handleComissionCommand ??= new RelayCommand(() =>
        {
            foreach (var item in MenuItems)
            {
                item.IsChecked = false;

            }

            var comissionMenuItem = MenuItems.FirstOrDefault(item => item.Name == "Provision");
            if (comissionMenuItem != null)
            {
                comissionMenuItem.IsChecked = true;
            }
            var handleComissionVM = new HandleComissionVM(this);
            var handleComissionUC = new HandleComissionUC
            {
                DataContext = handleComissionVM
            };
            CurrentView = handleComissionUC;
        });

        private ICommand _handleInsurancesCommand = null!;

        public ICommand HandleInsurancesCommand => _handleInsurancesCommand ??= new RelayCommand(() =>
        {
            foreach (var item in MenuItems)
            {
                item.IsChecked = false;

            }

            var insuranceMenuItem = MenuItems.FirstOrDefault(item => item.Name == "Försäkring");
            if (insuranceMenuItem != null)
            {
                insuranceMenuItem.IsChecked = true;
            }

            var handleInsurancesVM = new HandleInsurancesVM(this);
            var handleInsurancesUC = new HandleInsurancesUC
            {
                DataContext = handleInsurancesVM
            };
            CurrentView = handleInsurancesUC;
        });

        private ICommand _handleCustomerCommand = null!;
        public ICommand HandleCustomerCommand => _handleCustomerCommand ??= new RelayCommand(() =>
        {
            foreach (var item in MenuItems)
            {
                item.IsChecked = false;

            }

            var customerMenuItem = MenuItems.FirstOrDefault(item => item.Name == "Kund");
            if (customerMenuItem != null)
            {
                customerMenuItem.IsChecked = true;
            }

            var handleCustomerVM = new HandleCustomerVM(this);
            var handleCustomerUC = new HandleCustomerUC
            {
                DataContext = handleCustomerVM
            };
            CurrentView = handleCustomerUC;
        });

        private ICommand _analyzeSalesStatisticsCommand = null!;
        public ICommand AnalyzeSalesStatisticsCommand => _analyzeSalesStatisticsCommand ??= new RelayCommand(() =>
        {

            foreach (var item in MenuItems)
            {
                item.IsChecked = false;

            }

            var statisticMenuItem = MenuItems.FirstOrDefault(item => item.Name == "Försäljningsstatistik");
            if (statisticMenuItem != null)
            {
                statisticMenuItem.IsChecked = true;
            }

            var analyzeSalesVM = new AnalyzeSalesVM(this);
            var analyzeSalesUC = new AnalyzeSalesUC
            {
                DataContext = analyzeSalesVM,
            };
            CurrentView = analyzeSalesUC;
        });
    }
}
