namespace AgroSolutions.Identity.Domain.Notifications;

public record Notification(NotificationType Type, IEnumerable<object>? @Params = default);
