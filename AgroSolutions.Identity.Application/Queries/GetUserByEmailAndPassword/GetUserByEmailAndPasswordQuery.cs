using AgroSolutions.Identity.Application.DTOs;
using MediatR;

namespace AgroSolutions.Identity.Application.Queries.GetUserByEmailAndPassword;

public record GetUserByEmailAndPasswordQuery(string Email, string Password) : IRequest<TokenDto?>;
