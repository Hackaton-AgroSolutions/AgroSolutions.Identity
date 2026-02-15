using AgroSolutions.Identity.Application.DTOs;
using MediatR;

namespace AgroSolutions.Identity.Application.Queries.AuthenticateUser;

public record AuthenticateUserQuery(string Email, string Password) : IRequest<TokenDto?>;
