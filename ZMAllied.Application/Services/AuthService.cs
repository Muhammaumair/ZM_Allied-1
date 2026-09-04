using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ZMAllied.Application.DTOs.Auth;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Identity;

namespace ZMAllied.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !user.IsActive || user.IsDeleted)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return null;

            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            var permissions = await _userRepository.GetUserPermissionsAsync(user.Id);

            var token = _jwtTokenGenerator.GenerateToken(user, roles, permissions);

            return new LoginResponseDto
            {
                AccessToken = token,
                ExpiresAt = System.DateTime.UtcNow.AddMinutes(60),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    Roles = roles,
                    Permissions = permissions
                }
            };
        }
    }
}
