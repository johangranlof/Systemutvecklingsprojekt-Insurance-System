using Business.Controllers;
using Entities;

namespace Presentation.ViewModels
{
    public class HandleComissionVM : BaseVM
    {
        private readonly MainVM _mainViewModel;
        private readonly EmployeeController _employeeController;
        private readonly BusinessInsuranceController _businessInsuranceController;
        private readonly PrivateInsuranceController _privateInsuranceController;

        public HandleComissionVM(MainVM mainViewModel)
        {
            _mainViewModel = mainViewModel;
            CurrentMonth = GetCurrentMonth();
            _employeeController = new EmployeeController();
            _businessInsuranceController = new BusinessInsuranceController();
            _privateInsuranceController = new PrivateInsuranceController();
            LoadSellers();
            CalculateCommission();
        }

        private string _currentMonth;
        public string CurrentMonth
        {
            get => _currentMonth;
            set
            {
                if (_currentMonth != value)
                {
                    _currentMonth = value;
                    OnPropertyChanged(nameof(CurrentMonth));
                }
            }
        }
        public string GetCurrentMonth()
        {
            var now = DateTime.Now;
            var firstDayOfCurrentMonth = new DateTime(now.Year, now.Month, 1);
            return $"Beräknas utifrån aktiva försäkringar vid datumet: {firstDayOfCurrentMonth:yyyy-MM-dd}";
        }

        private List<EmployeeRole> _employeeRoles = new List<EmployeeRole>();
        public List<EmployeeRole> EmployeeRoles
        {
            get => _employeeRoles;
            set
            {
                if (_employeeRoles != value)
                {
                    _employeeRoles = value;
                    OnPropertyChanged(nameof(EmployeeRoles));
                }
            }
        }

        private void LoadSellers()
        {
            _employeeController.GetAllEmployeeRolesWithAgentNumber().ForEach(EmployeeRoles.Add);
        }

        private void CalculateCommission()
        {
            const decimal commissionRate = 0.12m;
            var allBusinessInsurances = _businessInsuranceController.GetAllBusinessInsurances();
            var allPrivateInsurances = _privateInsuranceController.GetAllPrivateInsurances();

            var now = DateTime.Now;
            var firstDayOfCurrentMonth = new DateTime(now.Year, now.Month, 1);

            var commissions = new List<(int agentNumber, decimal commission)>();
            var employeeRoles = _employeeController.GetAllEmployeeRolesWithAgentNumber();

            //Uträkning för provision baserat på aktiva försäkringar den 1a i pågående månad för varje säljare
            foreach (var role in employeeRoles)
            {
                var agentNumber = role.AgentNumber;

                var activeBusinessPremium = allBusinessInsurances
                    .Where(bi => bi.User != null &&
                                 bi.User.Employee != null &&
                                 bi.User.Employee.EmployeeRoles != null &&
                                 bi.User.Employee.EmployeeRoles.Any(er => er.AgentNumber == agentNumber) &&
                                 bi.InsuranceStatusId != 3 &&
                                 bi.StartDate <= firstDayOfCurrentMonth &&
                                 bi.EndDate >= firstDayOfCurrentMonth)
                    .Sum(bi => bi.Premium);

                var activePrivatePremium = allPrivateInsurances
                    .Where(pi => pi.User != null &&
                                 pi.User.Employee != null &&
                                 pi.User.Employee.EmployeeRoles != null &&
                                 pi.User.Employee.EmployeeRoles.Any(er => er.AgentNumber == agentNumber) &&
                                 pi.InsuranceStatusId != 3 &&
                                 pi.StartDate <= firstDayOfCurrentMonth &&
                                 pi.EndDate >= firstDayOfCurrentMonth)
                    .Sum(pi => pi.Premium);

                var totalMonthlyPremium = activeBusinessPremium + activePrivatePremium;

                var commission = totalMonthlyPremium * commissionRate;

                var employee = _employeeController.GetEmployeeByAgentNumber(agentNumber);
                if (employee != null)
                {
                    employee.Comission = commission;
                    _employeeController.UpdateEmployee(employee);
                }
            }
        }

    }
}