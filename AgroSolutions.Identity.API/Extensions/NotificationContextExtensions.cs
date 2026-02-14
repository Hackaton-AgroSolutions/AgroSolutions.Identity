using AgroSolutions.Identity.Domain.Notifications;
using AgroSolutions.Identity.Infrastructure.Extensions;

namespace AgroSolutions.Identity.API.Extensions;

public static partial class NotificationContextExtensions
{
    extension(INotificationContext notificationContext)
    {
        public IEnumerable<string> AsListString
            => notificationContext.Notifications.Select(n => string.Format(n.Type.GetDescription(), args: n?.Params?.ToArray() ?? []));
    }
}
