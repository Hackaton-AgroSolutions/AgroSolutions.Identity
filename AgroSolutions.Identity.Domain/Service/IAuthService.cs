using AgroSolutions.Identity.Domain.Entities;

namespace AgroSolutions.Identity.Domain.Service;

public interface IAuthService
{
    public string GenerateToken(User user);
}
