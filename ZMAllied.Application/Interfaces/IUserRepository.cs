using System.Collections.Generic;
using System.Threading.Tasks;
using ZMAllied.Domain.Entities.Identity;

namespace ZMAllied.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<List<string>> GetUserRolesAsync(int userId);
        Task<List<string>> GetUserPermissionsAsync(int userId);
    }
}
