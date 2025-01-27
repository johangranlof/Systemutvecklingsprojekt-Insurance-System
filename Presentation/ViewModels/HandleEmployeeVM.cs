using Business.Controllers;
using Entities;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class HandleEmployeeVM : BaseVM
    {
        private readonly MainVM _mainViewModel;
        private readonly EmployeeController _employeeController;

        public HandleEmployeeVM(MainVM mainViewModel)
        {
            _employeeController = new EmployeeController();
            _mainViewModel = mainViewModel;
            Employees = new ObservableCollection<Employee>();
            LoadEmployees();
        }

        private ObservableCollection<Employee> _employees;
        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }

        private void LoadEmployees()
        {
            _employeeController.GetEmployees().ForEach(Employees.Add);
        }

        private ICommand _showEmployeeDetailsCommand;
        public ICommand ShowEmployeeDetailsCommand => _showEmployeeDetailsCommand ??= new RelayCommand<Employee>(ShowEmployeeDetails);

        private void ShowEmployeeDetails(Employee employee)
        {
            if (employee != null)
            {
                var employeeDetailsVM = new EmployeeDetailsVM(_mainViewModel, employee);
                var employeeDetailsUC = new EmployeeDetailsUC { DataContext = employeeDetailsVM };
                _mainViewModel.CurrentView = employeeDetailsUC;
            }
        }

        private ICommand _addEmployeeViewCommand;
        public ICommand AddEmployeeViewCommand => _addEmployeeViewCommand ??= new RelayCommand(() =>
        {
            var addEmployeeVM = new AddEmployeeVM(_mainViewModel);
            var addEmployeeUC = new AddEmployeeUC { DataContext = addEmployeeVM };
            _mainViewModel.CurrentView = addEmployeeUC;
        });
    }
}