using AgroSolutions.Identity.Application.DTOs;
using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Notifications;
using AgroSolutions.Identity.Domain.Service;
using AgroSolutions.Identity.Infrastructure.Persistence;
using MediatR;
using Serilog;

namespace AgroSolutions.Identity.Application.Queries.AuthenticateUser;

public class AuthenticateUserQueryHandler(IUnitOfWork unitOfWork, IAuthService authService, INotificationContext notification) : IRequestHandler<AuthenticateUserQuery, TokenDto?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuthService _authService = authService;
    private readonly INotificationContext _notification = notification;

    public async Task<TokenDto?> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        Log.Information("Starting user login.");

        User? user = await _unitOfWork.Users.GetByEmailNoTrackingAsync(request.Email, cancellationToken);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            Log.Warning("Non-existent email and/or password combination.");
            _notification.AddNotification(NotificationType.InvalidCredentialsError);
            return null;
        }

        Log.Information("Generating a token for the user with ID {UserId} upon login.", user.UserId);
        string token = _authService.GenerateToken(user);

        Log.Information("Finished the user login.");
        return new(token);
    }
}
