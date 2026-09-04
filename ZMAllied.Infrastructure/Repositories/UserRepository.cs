using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Identity;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ZMAlliedDbContext _context;

        public UserRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Where(ur => ur.Role.IsActive && !ur.Role.IsDeleted)
                .Select(ur => ur.Role.Name)
                .ToListAsync();
        }

        public async Task<List<string>> GetUserPermissionsAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Where(rp => !rp.IsDeleted)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();
        }
    }
}
