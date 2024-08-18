namespace UserService.Models;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public User(int userId, string username, string password, string role)
    {
        UserId = userId;
        Username = username;
        Password = password;
        Role = role;
    }
}

public static class PredefinedUsers
{
    public static readonly List<User> Users =
    [
        new User(1, "admin", "admin123", "Admin"),
        new User(2, "user1", "user123", "User"),
        new User(3, "user2", "user234", "User")
    ];
}

public class LoginModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}