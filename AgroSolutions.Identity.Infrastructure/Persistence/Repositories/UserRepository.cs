using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Identity.Infrastructure.Persistence.Repositories;

public class UserRepository(AgroSolutionsIdentityDbContext dbContext) : IUserRepository
{
    private readonly AgroSolutionsIdentityDbContext _dbContext = dbContext;

    public async Task CreateAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public void Delete(User user) => _dbContext.Users.Remove(user);

    public Task<bool> ExistsByEmailExceptByUserIdAsync(string email, int userId, CancellationToken cancellationToken) => _dbContext.Users
        .AnyAsync(u => u.Email.ToUpper() == email.ToUpper() && u.UserId != userId, cancellationToken);

    public Task<User?> GetByEmailNoTrackingAsync(string email, CancellationToken cancellationToken) => _dbContext.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

    public Task<User?> GetByIdTrackingAsync(int userId, CancellationToken cancellationToken) => _dbContext.Users
        .FirstOrDefaultAsync(user => user.UserId == userId, cancellationToken);

    public Task<User?> GetByIdNoTrackingAsync(int userId, CancellationToken cancellationToken) => _dbContext.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(user => user.UserId == userId, cancellationToken);

    public Task<bool> IsEmailInUseAsync(string email, CancellationToken cancellationToken) => _dbContext.Users
        .AnyAsync(user => user.Email.ToUpper() == email.ToUpper(), cancellationToken);
}
