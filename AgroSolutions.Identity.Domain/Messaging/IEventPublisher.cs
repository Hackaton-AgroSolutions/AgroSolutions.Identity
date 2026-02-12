using AgroSolutions.Identity.Domain.Common;

namespace AgroSolutions.Identity.Domain.Messaging;

public interface IEventPublisher
{
    Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken, string? correlationId = default);
}
