using Data;
using Data.Repositories;
using Entities;

namespace Business.Controllers
{
    public class UserController
    {
        private readonly UnitOfWork _unitOfWork;

        public UserController()
        {
            _unitOfWork = new UnitOfWork(new AppDbContext());
        }

        public User AddUser(User user)
        {
            _unitOfWork.UserRepository.Add(user);
            _unitOfWork.Save();
            return user;
        }

        public void RemoveUser(User user)
        {
            _unitOfWork.UserRepository.Delete(user);
            _unitOfWork.Save();
        }

        public User UpdateUser(User user)
        {
            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.Save();
            return user;
        }

        public List<int> GetEmploymentNumbers()
        {
            List<int> employmentNumbers = new List<int>();
            foreach (var employee in _unitOfWork.EmployeeRepository.GetAll())
            {
                employmentNumbers.Add(employee.EmployeeNumber);
            }
            return employmentNumbers;
        }

        public List<User> GetUsers()
        {
            List<User> users = new List<User>();
            foreach (var user in _unitOfWork.UserRepository.GetAll())
            {
                users.Add(user);
            }
            return users;
        }

        public User GetUserById(int id)
        {
            return _unitOfWork.UserRepository.GetById(id);
        }
    }
}
