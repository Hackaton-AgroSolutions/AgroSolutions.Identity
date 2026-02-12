using AgroSolutions.Identity.Domain.Common;

namespace AgroSolutions.Identity.Domain.Events;

public record DeletedUserEvent(int UserId) : IDomainEvent;
