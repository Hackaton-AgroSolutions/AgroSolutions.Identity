using MediatR;

namespace AgroSolutions.Identity.Application.Commands.DeleteUser;

public record DeleteUserCommand(int UserId) : IRequest<Unit?>;
