namespace AgroSolutions.Identity.Domain.Entities;

public class UserEvent(int userId, string name, string email)
{
    public int UserEventId { get; private set; }
    public int UserId { get; private set; } = userId;
    public User User { get; private set; } = null!;
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
    public DateTime EventAt { get; private set; }

    public static UserEvent FromUser(User user) => new(user.UserId, user.Name, user.Email);
}
