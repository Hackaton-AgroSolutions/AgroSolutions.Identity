using MediatR;

namespace AgroSolutions.Identity.Application.Queries.GetUserById;

public record GetUserByIdQuery(int UserId) : IRequest<GetUserByIdResult?>;
