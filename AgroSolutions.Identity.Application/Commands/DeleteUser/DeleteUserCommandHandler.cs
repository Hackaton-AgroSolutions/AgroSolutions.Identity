using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Events;
using AgroSolutions.Identity.Domain.Messaging;
using AgroSolutions.Identity.Domain.Notifications;
using AgroSolutions.Identity.Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace AgroSolutions.Identity.Application.Commands.DeleteUser;

public class DeleteUserCommandHandler(
    INotificationContext notification,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IMemoryCache memoryCache) : IRequestHandler<DeleteUserCommand, Unit?>
{
    private readonly INotificationContext _notification = notification;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task<Unit?> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        Log.Information("Initiating user deletion.");

        User? user = await _unitOfWork.Users.GetByIdTrackingAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            Log.Warning("User with ID {UserId} ​​not found.", request.UserId);
            _notification.AddNotification(NotificationType.UserNotFound);
            return null;
        }

        Log.Information("Initiating the process of deleting the user with ID {UserId} from the database and adding them to the queue.", request.UserId);
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        _unitOfWork.Users.Delete(user);
        await _eventPublisher.PublishAsync(new DeletedUserEvent(user.UserId), cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        Log.Information("Removing the user ID {UserId} in Cache with same key from user ID to futures validations of token.", user.UserId);
        _memoryCache.Remove(user.UserId);

        Log.Information("User deletion process with full ID {UserId}.", user.UserId);
        return Unit.Value;
    }
}
