using CCMS.Domain.Entities;

namespace CCMS.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
