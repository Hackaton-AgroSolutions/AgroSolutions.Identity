using AgroSolutions.Identity.Application.DTOs;
using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Notifications;
using AgroSolutions.Identity.Domain.Service;
using AgroSolutions.Identity.Infrastructure.Persistence;
using MediatR;
using Serilog;

namespace AgroSolutions.Identity.Application.Commands.CreateUser;

public class CreateUserCommandHandler(INotificationContext notification, IUnitOfWork unitOfWork, IAuthService authService) : IRequestHandler<CreateUserCommand, TokenDto?>
{
    private readonly INotificationContext _notification = notification;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuthService _authService = authService;

    public async Task<TokenDto?> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Starting user creation.");

        if (await _unitOfWork.Users.IsEmailInUseAsync(request.Email, cancellationToken))
        {
            Log.Warning("The email address {Email} is already in use.", request.Email);
            _notification.AddNotification(NotificationType.EmailAlreadyInUse);
            return null;
        }

        Log.Information("Adding the new user to the database.");
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        User user = new(request.Name, request.Email, BCrypt.Net.BCrypt.HashPassword(request.Password));
        await _unitOfWork.Users.CreateAsync(user, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        Log.Information("Generating a new token for the updated data for the user with ID {UserId}.", user.UserId);
        string token = _authService.GenerateToken(user);

        return new(token);
    }
}
