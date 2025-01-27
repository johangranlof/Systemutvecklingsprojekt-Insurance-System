using Data;
using Data.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Business.Controllers
{
    public class EmployeeController
    {
        private readonly UnitOfWork _unitOfWork;

        public EmployeeController()
        {
            _unitOfWork = new UnitOfWork(new AppDbContext());
        }

        public List<Role> GetRoles()
        {
            return _unitOfWork.RoleRepository.GetAll().ToList();
        }

        public int GenerateUniqueAgentNumber()
        {
            Random random = new Random();
            int agentNumber;

            do
            {
                agentNumber = random.Next(1000, 10000);
            }
            while (_unitOfWork.EmployeeRoleRepository.FindByCondition(er => er.AgentNumber == agentNumber).Any());

            return agentNumber;
        }

        public Employee AddEmployee(Employee employee)
        {
            _unitOfWork.EmployeeRepository.Add(employee);
            _unitOfWork.Save();


            return employee;
        }

        public void AddEmployeeRole(EmployeeRole employeeRole)
        {
            _unitOfWork.EmployeeRoleRepository.Add(employeeRole);
            _unitOfWork.Save();
        }

        public void UpdateEmployee(Employee employee)
        {
            _unitOfWork.EmployeeRepository.Update(employee);
            _unitOfWork.Save();
        }

        public List<Employee> GetEmployees()
        {
            List<Employee> employees = new List<Employee>();
            foreach (var employee in _unitOfWork.EmployeeRepository.GetAll())
            {
                employees.Add(employee);
            }
            return employees;
        }

        public void RemoveEmployee(Employee employee)
        {
            _unitOfWork.EmployeeRepository.Delete(employee);
            _unitOfWork.Save();
        }

        public Employee GetEmployeeByEmployeeNumber(int employeeNumber)
        {
            return _unitOfWork.EmployeeRepository.GetById(employeeNumber);
        }

        public int GetAgentNumberByEmployeeNumber(int employeeNumber)
        {
            var employee = _unitOfWork.EmployeeRepository
                .FindByCondition(e => e.EmployeeNumber == employeeNumber)
                .FirstOrDefault();

            var employeeRole = _unitOfWork.EmployeeRoleRepository
                .FindByCondition(er => er.EmployeeNumber == employeeNumber)
                .FirstOrDefault();

            return employeeRole?.AgentNumber ?? 0;
        }

        public List<EmployeeRole> GetEmployeeRolesByEmployeeNumber(int employeeNumber)
        {
            var employeeRoles = _unitOfWork.EmployeeRoleRepository
                .FindByCondition(er => er.Employee.EmployeeNumber == employeeNumber)
                .ToList();

            foreach (var employeeRole in employeeRoles)
            {
                if (employeeRole != null)
                {
                    var role = _unitOfWork.RoleRepository
                        .FindByCondition(r => r.Id == employeeRole.RoleId)
                        .FirstOrDefault();

                    if (role != null)
                    {
                        employeeRole.Role = role;
                    }
                }
            }
            return employeeRoles;
        }

        public bool IsSsnExists(string SSN)
        {
            return _unitOfWork.EmployeeRepository
                .FindByCondition(pc => pc.SSN == SSN)
                .Any();
        }

        public List<Employee> GetAllEmployeesWithSalesRole()
        {
            var employeeNumbersWithSalesRole = _unitOfWork.EmployeeRoleRepository
                .FindByCondition(er => er.RoleId == 1)
                .Select(er => er.EmployeeNumber)
                .ToList();

            var employees = _unitOfWork.EmployeeRepository
                .FindByCondition(e => employeeNumbersWithSalesRole.Contains(e.EmployeeNumber))
                .ToList();

            return employees;
        }
        public List<EmployeeRole> GetAllEmployeeRolesWithAgentNumber()
        {
            var allEmployeeRoles = _unitOfWork.EmployeeRoleRepository.GetAll();
            var employeeRolesWithAgentNumber = allEmployeeRoles
                .Where(er => er.AgentNumber > 0)
                .Include(er => er.Employee)
                .ToList();

            return employeeRolesWithAgentNumber;
        }

        public Employee GetEmployeeByAgentNumber(int agentNumber)
        {
            return _unitOfWork.EmployeeRepository
                .FindByCondition(e => e.EmployeeRoles.Any(er => er.AgentNumber == agentNumber))
                .FirstOrDefault();
        }
        public List<User> GetUsersByEmployeeNumber(int employeeNumber)
        {
            return _unitOfWork.UserRepository.FindByCondition(u => u.EmployeeNumber == employeeNumber).ToList();
        }
    }
}
