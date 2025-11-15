using Server.Models;

namespace Server.Data;

public static class UserStore
{
    public static List<User> Users = new()
    {
        new User { Id = 1, Username = "admin", PasswordHash = "admin123", Role = "Admin" },
        new User { Id = 2, Username = "user", PasswordHash = "user123", Role = "User" }
    };
}