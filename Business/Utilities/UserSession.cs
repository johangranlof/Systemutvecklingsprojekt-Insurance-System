// UserSession-klassen hanterar en användarsession som en singleton, vilket säkerställer att det bara finns en aktiv session för en användare åt gången.
// Den låter en sätta och hämta användarinformation under en session.

using Entities;

namespace Business.Utilities
{
    public class UserSession
    {
        private static UserSession _instance;

        public User? User { get; private set; }

        private UserSession() { }

        public static UserSession GetInstance()
        {
            if (_instance == null)
            {
                _instance = new UserSession();
            }
            return _instance;
        }

        public void SetUser(User user)
        {
            User = user;
        }
    }
}
