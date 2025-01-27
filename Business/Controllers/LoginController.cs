using Business.Utilities;
using Data;
using Data.Repositories;
using Entities;

namespace Business.Controllers
{
    public class LoginController
    {
        private readonly UnitOfWork _unitOfWork;

        public LoginController()
        {
            _unitOfWork = new UnitOfWork(new AppDbContext());
        }

        public User? Authenticate(string username, string password)
        {
            var user = _unitOfWork.UserRepository.FindByCondition(u => u.Username == username).FirstOrDefault();

            if (user != null && user.Password == password)
            {
                var userSession = UserSession.GetInstance();
                userSession.SetUser(user);

                return user;
            }
            return null;
        }

    }
}
