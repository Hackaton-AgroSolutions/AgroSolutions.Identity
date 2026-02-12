using AgroSolutions.Identity.Application.DTOs;
using MediatR;

namespace AgroSolutions.Identity.Application.Commands.UpdateUser;

public record UpdateUserCommand(int UserId, string Name, string Email) : IRequest<TokenDto?>;
