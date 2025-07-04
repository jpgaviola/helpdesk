using HelpdeskBlazor.Models;

namespace HelpdeskBlazor.Services
{
    public interface ISessionService
    {
        User? CurrentUser { get; }
        bool IsAuthenticated { get; }
        void SetCurrentUser(User user);
        void ClearSession();
        event Action? OnUserChanged;
    }

    public class SessionService : ISessionService
    {
        private User? _currentUser;
        public User? CurrentUser => _currentUser;
        public bool IsAuthenticated => _currentUser != null;
        public event Action? OnUserChanged;

        public void SetCurrentUser(User user)
        {
            _currentUser = user;
        }

        public void ClearSession()
        {
            _currentUser = null;
        }
    }
}