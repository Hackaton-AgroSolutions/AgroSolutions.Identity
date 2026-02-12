namespace AgroSolutions.Identity.Domain.Entities;

public class User
{
    public int UserId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }

    public User(int userId, string name, string email, string? password = default)
    {
        UserId = userId;
        Name = name;
        Email = email;
        Password = password ?? string.Empty;
    }

    public User(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public void Update(string name, string email)
    {
        Name = name;
        Email = email;
    }
}
