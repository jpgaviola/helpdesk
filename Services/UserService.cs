using Microsoft.EntityFrameworkCore;
using HelpdeskBlazor.Data;
using HelpdeskBlazor.Models;

namespace HelpdeskBlazor.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<List<User>> SearchUsersAsync(string searchTerm);
        Task<List<User>> GetUsersByStatusAsync(string status);
        Task<List<User>> GetUsersByRoleAsync(string role);
        Task<List<User>> GetUsersByDepartmentAsync(string department);
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
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
            user.CreatedDate = DateTime.Now;
            user.IsDeleted = false;
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            user.ModifiedDate = DateTime.Now;
            
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
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
            var query = _context.Users.Where(u => !u.IsDeleted && u.Email == email);
            
            if (excludeUserId.HasValue)
                query = query.Where(u => u.Id != excludeUserId.Value);
            
            return await query.AnyAsync();
        }
    }
}