using Microsoft.EntityFrameworkCore;
using HelpdeskBlazor.Data;
using HelpdeskBlazor.Models;
using BCrypt.Net;

namespace HelpdeskBlazor.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<User> CreateUserWithPasswordAsync(string name, string email, string role, string department, string plainTextPassword, string? phone = null, string? location = null, string status = "Active");
        Task<User> UpdateUserAsync(User user);
        Task<bool> UpdateUserPasswordAsync(int userId, string newPassword);
        Task<bool> DeleteUserAsync(int id);
        Task<List<User>> SearchUsersAsync(string searchTerm);
        Task<List<User>> GetUsersByStatusAsync(string status);
        Task<List<User>> GetUsersByRoleAsync(string role);
        Task<List<User>> GetUsersByDepartmentAsync(string department);
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
        Task<bool> VerifyPasswordAsync(int userId, string password);
    }

    public class UserService : IUserService
    {
        private readonly HelpdeskDbContext _context;

        public UserService(HelpdeskDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Where(u => !u.IsDeleted)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id && !u.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            // This method should NOT be used directly when passwords are involved
            // Use CreateUserWithPasswordAsync instead
            user.CreatedDate = DateTime.Now;
            user.IsDeleted = false;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Creates a new user with properly hashed password
        /// </summary>
        public async Task<User> CreateUserWithPasswordAsync(string name, string email, string role, string department, string plainTextPassword, string? phone = null, string? location = null, string status = "Active")
        {
            // Validate password strength
            if (!IsValidPassword(plainTextPassword))
            {
                throw new ArgumentException("Password must be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, and one number.");
            }

            // Check if email already exists
            bool emailExists = await EmailExistsAsync(email);
            if (emailExists)
            {
                throw new InvalidOperationException($"Email '{email}' already exists.");
            }

            // Hash the password using BCrypt - CORRECTED NAMESPACE
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainTextPassword);

            var user = new User
            {
                Name = name.Trim(),
                Email = email.Trim().ToLower(),
                Role = role,
                Department = department,
                Status = status,
                Phone = phone?.Trim(),
                Location = location?.Trim(),
                PasswordHash = hashedPassword,
                LastPasswordChange = DateTime.Now,
                CreatedDate = DateTime.Now,
                IsDeleted = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Created user: {user.Name} with hashed password");
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            user.ModifiedDate = DateTime.Now;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Updates a user's password with proper hashing
        /// </summary>
        public async Task<bool> UpdateUserPasswordAsync(int userId, string newPassword)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) return false;

            // Validate password strength
            if (!IsValidPassword(newPassword))
            {
                throw new ArgumentException("Password must be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, and one number.");
            }

            // Hash the new password - CORRECTED NAMESPACE
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.LastPasswordChange = DateTime.Now;
            user.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null) return false;

            // Soft delete
            user.IsDeleted = true;
            user.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return await GetAllUsersAsync();

            return await _context.Users
                .Where(u => !u.IsDeleted &&
                    (u.Name.Contains(searchTerm) ||
                     u.Email.Contains(searchTerm) ||
                     u.Department.Contains(searchTerm)))
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByStatusAsync(string status)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted && u.Status == status)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByRoleAsync(string role)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted && u.Role == role)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByDepartmentAsync(string department)
        {
            return await _context.Users
                .Where(u => !u.IsDeleted && u.Department == department)
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
        {
            var query = _context.Users.Where(u => !u.IsDeleted && u.Email.ToLower() == email.ToLower());

            if (excludeUserId.HasValue)
                query = query.Where(u => u.Id != excludeUserId.Value);

            return await query.AnyAsync();
        }

        /// <summary>
        /// Verifies a password against the stored hash
        /// </summary>
        public async Task<bool> VerifyPasswordAsync(int userId, string password)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                return false;

            // CORRECTED NAMESPACE
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        /// <summary>
        /// Validates password strength
        /// </summary>
        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
                return false;

            // Check for at least one uppercase, one lowercase, and one digit
            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);

            return hasUpper && hasLower && hasDigit;
        }
    }
}