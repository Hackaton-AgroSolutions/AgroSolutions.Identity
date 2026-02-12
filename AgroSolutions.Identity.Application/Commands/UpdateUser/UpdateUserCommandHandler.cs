using AgroSolutions.Identity.Application.DTOs;
using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Notifications;
using AgroSolutions.Identity.Domain.Service;
using AgroSolutions.Identity.Infrastructure.Persistence;
using MediatR;
using Serilog;

namespace AgroSolutions.Identity.Application.Commands.UpdateUser;

public class UpdateUserCommandHandler(INotificationContext notification, IUnitOfWork unitOfWork, IAuthService authService) : IRequestHandler<UpdateUserCommand, TokenDto?>
{
    private readonly INotificationContext _notification = notification;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAuthService _authService = authService;

    public async Task<TokenDto?> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Starting the user data update.");

        User? user = await _unitOfWork.Users.GetByIdTrackingAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            Log.Warning("User with ID {UserId} ​​not found.", request.UserId);
            _notification.AddNotification(NotificationType.UserNotFound);
            return null;
        }

        Log.Information("Checking availability of the new email address {Email}.", request.Email);
        if (await _unitOfWork.Users.ExistsByEmailExceptByUserIdAsync(request.Email, request.UserId, cancellationToken))
        {
            Log.Warning("Email {Email} already in use.", request.UserId);
            _notification.AddNotification(NotificationType.EmailAlreadyInUse);
            return null;
        }

        Log.Information("Initiating update of user data with ID {UserId} in the database.", request.Email);
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        user.Update(request.Name, request.Email);
        Log.Information("Generating a new token given the updated user ID {UserId}.", request.Email);
        string token = _authService.GenerateToken(user);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new(token);
    }
}
