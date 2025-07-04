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

        public async Task<User?> AuthenticateAsync(string emailOrUsername, string password)
        {
            if (string.IsNullOrEmpty(emailOrUsername) || string.IsNullOrEmpty(password))
                return null;

            // Try to find user by email first, then by name
            var user = await _context.Users
                .Where(u => !u.IsDeleted && u.Status == "Active" &&
                           (u.Email.ToLower() == emailOrUsername.ToLower() ||
                            u.Name.ToLower() == emailOrUsername.ToLower()))
                .FirstOrDefaultAsync();

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                return null;

            // Verify password using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (!isPasswordValid)
                return null;

            // Update last login
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

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}