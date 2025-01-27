using Business.Controllers;
using Entities;
using Presentation.Views.Controls;
using PresentationLayer.Commands;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Presentation.ViewModels
{
    public class RemoveEmployeeVM : BaseVM
    {
        private readonly MainVM _mainViewModel;
        private readonly Window _popupWindow;
        private Employee _employee;
        public EmployeeDetailsVM _emploeeDetailsVM;
        private readonly EmployeeController _employeeController;
        private ObservableCollection<Employee> _employees;
        public RemoveEmployeeVM(Employee employee, Window popupWindow, MainVM mainViewModel, EmployeeDetailsVM employeeDetailsVM, EmployeeController employeeController)
        {
            _employee = employee;
            _popupWindow = popupWindow;
            _mainViewModel = mainViewModel;
            _emploeeDetailsVM = employeeDetailsVM;
            _employeeController = employeeController;

        }
        public Employee Employee
        {
            get => _employee;
            set
            {
                _employee = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Employee> Employees
        {
            get => _employees ??= new ObservableCollection<Employee>();
            set
            {
                _employees = value;
                OnPropertyChanged();
            }
        }

        private ICommand _removeEmployeeCommand;
        public ICommand RemoveEmployeeCommand => _removeEmployeeCommand ??= new RelayCommand<Employee>(RemoveEmployee);
        private void RemoveEmployee(Employee employee)
        {
            if (employee != null)
            {
                _employeeController.RemoveEmployee(employee);
                Employees.Remove(employee);
                _mainViewModel.CurrentView = new HandleEmployeeUC
                {
                    DataContext = new HandleEmployeeVM(_mainViewModel)
                };

            }
        }

        private ICommand _confirmCommand;
        public ICommand ConfirmCommand => _confirmCommand ??= new RelayCommand(() =>
        {
            RemoveEmployee(_employee);
            _popupWindow.Close();
        });

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(() =>
        {
            _popupWindow.Close();

        });
    }
}
