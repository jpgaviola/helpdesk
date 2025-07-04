using Microsoft.EntityFrameworkCore;
using HelpdeskBlazor.Data;
using HelpdeskBlazor.Models;
using BCrypt.Net;

namespace HelpdeskBlazor.Services
{
    public class AuthService : IAuthService
    {
        private readonly HelpdeskDbContext _context;

        public AuthService(HelpdeskDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string emailOrUsername, string password, string domain)
        {
            if (string.IsNullOrEmpty(emailOrUsername) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(domain))
                return null;

            var user = await _context.Users
                .Where(u => !u.IsDeleted && u.Status == "Active" &&
                           u.Domain == domain &&
                           (u.Email.ToLower() == emailOrUsername.ToLower() ||
                            u.Name.ToLower() == emailOrUsername.ToLower()))
                .FirstOrDefaultAsync();

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                return null;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
                return null;

            await UpdateLastLoginAsync(user.Id);

            return user;
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted && u.Email.ToLower() == email.ToLower())
                .AnyAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted && u.Email.ToLower() == email.ToLower())
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByEmailAndDomainAsync(string email, string domain)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted &&
                           u.Email.ToLower() == email.ToLower() &&
                           u.Domain == domain)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetAvailableDomainsAsync()
        {
            var domains = await _context.Users
                .Where(u => !u.IsDeleted && !string.IsNullOrEmpty(u.Domain))
                .Select(u => u.Domain!)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return domains;
        }
    }
}