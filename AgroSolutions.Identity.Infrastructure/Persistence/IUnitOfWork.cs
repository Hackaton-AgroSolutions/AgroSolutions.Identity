using AgroSolutions.Identity.Domain.Repositories;

namespace AgroSolutions.Identity.Infrastructure.Persistence;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    Task BeginTransactionAsync(CancellationToken cancellationToken);
    Task CommitAsync(CancellationToken cancellationToken);
}
