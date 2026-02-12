using System.ComponentModel;

namespace AgroSolutions.Identity.Domain.Messaging;

public enum EventType : byte
{
    [Description("DELETED_USER")] DeletedUser
}
