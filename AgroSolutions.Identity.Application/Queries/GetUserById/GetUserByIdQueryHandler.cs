using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Notifications;
using AgroSolutions.Identity.Infrastructure.Persistence;
using MediatR;
using Serilog;

namespace AgroSolutions.Identity.Application.Queries.GetUserById;

public class GetUserByIdQueryHandler(IUnitOfWork unitOfWork, INotificationContext notification) : IRequestHandler<GetUserByIdQuery, GetUserByIdResult?>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly INotificationContext _notification = notification;

    public async Task<GetUserByIdResult?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        Log.Information("Starting the search for user data by ID.");

        User? user = await _unitOfWork.Users.GetByIdNoTrackingAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            Log.Warning("User with ID {UserId} ​​not found.", request.UserId);
            _notification.AddNotification(NotificationType.UserNotFound);
            return null;
        }

        Log.Information("User with ID {UserId} found.", request.UserId);
        return new(user.UserId, user.Name, user.Email);
    }
}
