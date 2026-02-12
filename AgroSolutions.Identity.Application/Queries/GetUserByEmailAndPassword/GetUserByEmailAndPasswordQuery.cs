using MediatR;
using AgroSolutions.Identity.Application.DTOs;

namespace AgroSolutions.Identity.Application.Queries.GetUserByEmailAndPassword;

public record GetUserByEmailAndPasswordQuery(string Email, string Password) : IRequest<TokenDto?>;
