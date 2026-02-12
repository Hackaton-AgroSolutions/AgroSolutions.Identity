using AgroSolutions.Identity.Application.DTOs;
using MediatR;

namespace AgroSolutions.Identity.Application.Commands.CreateUser;

public record CreateUserCommand(string Name, string Email, string Password) : IRequest<TokenDto?>;
