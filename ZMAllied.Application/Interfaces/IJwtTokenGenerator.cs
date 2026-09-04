using System.Collections.Generic;
using ZMAllied.Domain.Entities.Identity;

namespace ZMAllied.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, List<string> roles, List<string> permissions);
    }
}
