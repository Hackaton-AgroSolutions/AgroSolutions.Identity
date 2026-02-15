using MediatR;

namespace AgroSolutions.Identity.Application.Queries.GetUser;

public record GetUserQuery(int UserId) : IRequest<GetUserQueryResult?>;
