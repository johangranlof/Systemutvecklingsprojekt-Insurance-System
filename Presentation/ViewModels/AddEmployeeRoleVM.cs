using Business.Controllers;
using Entities;
using Presentation.ViewModels;
using PresentationLayer.Commands;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

public class AddEmployeeRoleVM : BaseVM
{
    private readonly MainVM _mainViewModel;
    private readonly Window _popupWindow;
    public ObservableCollection<EmployeeRole> EmployeeRoles { get; set; }
    private EmployeeController _employeeController { get; set; }
    public Employee Employee = new Employee();

    public ObservableCollection<Role> Roles { get; set; }
    public ObservableCollection<Role> FilteredRoles { get; set; } // Ny lista för filtrerade roller

    public AddEmployeeRoleVM(Window popupWindow, MainVM mainViewModel, Employee employee)
    {
        _popupWindow = popupWindow;
        _mainViewModel = mainViewModel;
        EmployeeRoles = new ObservableCollection<EmployeeRole>();
        _employeeController = new EmployeeController();
        Roles = new ObservableCollection<Role>();
        FilteredRoles = new ObservableCollection<Role>();
        Employee = employee;

        LoadData();
    }

    private Role _selectedRole;
    public Role SelectedRole
    {
        get => _selectedRole;
        set
        {
            _selectedRole = value;
            OnPropertyChanged();
        }
    }

    private int _selectedPercentage;
    public int SelectedPercentage
    {
        get => _selectedPercentage;
        set
        {
            _selectedPercentage = value;
            OnPropertyChanged();
        }
    }

    private readonly List<int> _employmentPercentages = new List<int> { 25, 50, 75, 100 };
    public List<int> EmploymentPercentages => _employmentPercentages;

    public void LoadData()
    {
        var employeeroles = _employeeController.GetEmployeeRolesByEmployeeNumber(Employee.EmployeeNumber);
        var roles = _employeeController.GetRoles();
        Roles.Clear();
        FilteredRoles.Clear();

        roles.ForEach(role =>
        {
            if (!employeeroles.Any(er => er.Role.Id == role.Id))
            {
                Roles.Add(role);
                FilteredRoles.Add(role);
            }
        });
    }

    private int _agentNumber;
    public int AgentNumber
    {
        get => _agentNumber;
        set
        {
            _agentNumber = value;
            OnPropertyChanged();
        }
    }

    private ICommand _confirmCommand;
    public ICommand ConfirmCommand => _confirmCommand ??= new RelayCommand(() =>
    {
        if (SelectedRole != null)
        {
            if (SelectedRole.Id == 1)
            {
                AgentNumber = _employeeController.GenerateUniqueAgentNumber();
            }

            var newEmployeeRole = new EmployeeRole
            {
                EmployeeNumber = Employee.EmployeeNumber,
                RoleId = SelectedRole.Id,
                EmploymentRate = SelectedPercentage,
                AgentNumber = AgentNumber
            };

            _employeeController.AddEmployeeRole(newEmployeeRole);

            EmployeeRoles.Add(newEmployeeRole);

            _popupWindow.Close();
        }
        else
        {
            MessageBox.Show("Välj en roll och sysselsättningsgrad för att spara.");
        }
    });

    private ICommand _cancelCommand;
    public ICommand CancelCommand => _cancelCommand ??= new RelayCommand(() =>
    {
        _popupWindow.Close();
    });
}
