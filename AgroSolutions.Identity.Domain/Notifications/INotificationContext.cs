namespace AgroSolutions.Identity.Domain.Notifications;

public interface INotificationContext
{
    bool HasNotifications { get; }
    bool HasValidations { get; }

    IReadOnlyCollection<Notification> Notifications { get; }
    Dictionary<string, string[]> Validations { get; }

    void AddNotification(NotificationType notificationCode, IEnumerable<object>? @params = default);
    void AddValidation(string field, string error);
}
