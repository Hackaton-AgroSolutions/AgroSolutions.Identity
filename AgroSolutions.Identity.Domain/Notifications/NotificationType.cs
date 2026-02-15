using System.ComponentModel;

namespace AgroSolutions.Identity.Domain.Notifications;

public enum NotificationType : byte
{
    [Description("The email address provided is already in use")] EmailAlreadyInUse,
    [Description("The email and/or password do not match")] InvalidCredentialsError,
    [Description("The user with ID {0} no longer exists.")] UserNoLongerExists,
    [Description("User not found")] UserNotFound
}
