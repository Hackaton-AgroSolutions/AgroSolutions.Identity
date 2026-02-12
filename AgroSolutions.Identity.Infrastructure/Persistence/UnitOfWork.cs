using AgroSolutions.Identity.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace AgroSolutions.Identity.Infrastructure.Persistence;

public class UnitOfWork(AgroSolutionsIdentityDbContext dbContext, IUserRepository users) : IUnitOfWork
{
    private readonly AgroSolutionsIdentityDbContext _dbContext = dbContext;
    private IDbContextTransaction? _transaction;

    public IUserRepository Users => users;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken) => _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _transaction!.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _transaction!.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _dbContext.Dispose();
    }
}
