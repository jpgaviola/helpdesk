using HelpdeskBlazor.Models;
namespace HelpdeskBlazor.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string emailOrUsername, string password, string domain);
        Task<bool> IsEmailExistsAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByEmailAndDomainAsync(string email, string domain);
        Task UpdateLastLoginAsync(int userId);
        Task<List<string>> GetAvailableDomainsAsync();
    }
}
