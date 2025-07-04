using HelpdeskBlazor.Models;

namespace HelpdeskBlazor.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string emailOrUsername, string password);
        Task<bool> IsEmailExistsAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task UpdateLastLoginAsync(int userId);
    }
}