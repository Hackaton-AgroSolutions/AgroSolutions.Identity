using MediatR;

namespace AgroSolutions.Identity.Application.Queries.ValidateToken;

public record ValidateTokenQuery(int UserId) : IRequest<Unit?>;
