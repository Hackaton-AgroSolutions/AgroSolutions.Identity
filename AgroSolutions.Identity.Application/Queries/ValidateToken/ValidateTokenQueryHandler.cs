using AgroSolutions.Identity.Domain.Entities;
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
            if (!await _unitOfWork.Users.ExistsByIdAsync(request.UserId, cancellationToken))
            {
                Log.Warning("The user with ID {UserId} no longer exists.", request.UserId);
                _notification.AddNotification(NotificationType.UserNoLongerExists);
                return null;
            }

            _memoryCache.Set(request.UserId, request.UserId);
        }

        return Unit.Value;
    }
}
