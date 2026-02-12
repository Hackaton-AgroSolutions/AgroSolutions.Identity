using AgroSolutions.Identity.Domain.Entities;

namespace AgroSolutions.Identity.Domain.Repositories;

public interface IUserRepository
{
    Task CreateAsync(User user, CancellationToken cancellationToken);
    void Delete(User user);
    Task<bool> ExistsByEmailExceptByUserIdAsync(string email, int userId, CancellationToken cancellationToken);
    Task<User?> GetByEmailNoTrackingAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByIdTrackingAsync(int userId, CancellationToken cancellationToken);
    Task<User?> GetByIdNoTrackingAsync(int userId, CancellationToken cancellationToken);
    Task<bool> IsEmailInUseAsync(string email, CancellationToken cancellationToken);
}