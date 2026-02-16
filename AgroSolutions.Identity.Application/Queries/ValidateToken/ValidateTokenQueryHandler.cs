using AgroSolutions.Identity.Domain.Notifications;
using AgroSolutions.Identity.Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace AgroSolutions.Identity.Application.Queries.ValidateToken;

public class ValidateTokenQueryHandler(IUnitOfWork unitOfWork, INotificationContext notification, IMemoryCache memoryCache) : IRequestHandler<ValidateTokenQuery, Unit?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly INotificationContext _notification = notification;
    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task<Unit?> Handle(ValidateTokenQuery request, CancellationToken cancellationToken)
    {
        int? userId = _memoryCache.Get<int?>(request.UserId);
        if (userId is null)
        {
            Log.Warning("User ID {UserId} not found in Cache with same value.", request.UserId);
            if (!await _unitOfWork.Users.ExistsByIdAsync(request.UserId, cancellationToken))
            {
                Log.Warning("The user with ID {UserId} no longer exists.", request.UserId);
                _notification.AddNotification(NotificationType.UserNoLongerExists);
                return null;
            }

            Log.Information("Inserting the user ID {UserId} in Cache with same key from user ID to futures validations of token.", request.UserId);
            _memoryCache.Set(request.UserId, request.UserId);
        }

        return Unit.Value;
    }
}
