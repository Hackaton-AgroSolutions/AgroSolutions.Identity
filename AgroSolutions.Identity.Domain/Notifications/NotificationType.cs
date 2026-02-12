using System.ComponentModel;

namespace AgroSolutions.Identity.Domain.Notifications;

public enum NotificationType : byte
{
    [Description("The email address provided is already in use")] EmailAlreadyInUse,
    [Description("The email and/or password do not match")] InvalidCredentialsError,
    [Description("User not found")] UserNotFound
}
