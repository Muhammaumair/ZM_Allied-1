using System.Threading.Tasks;
using ZMAllied.Application.DTOs.Auth;

namespace ZMAllied.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    }
}
