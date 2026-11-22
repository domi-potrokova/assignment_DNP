// File: Entities/User.cs
namespace Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

    // Navigation properties: a user has many posts and many comments.
    // Initialised to empty lists so they are never null when you add to them.
    public List<Post> Posts { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();

    // Private no-argument constructor: EFC uses it when it builds objects from database rows
    private User() { }

    public User(string userName, string password, int id)
    {
        UserName = userName;
        Password = password;
        Id = id; // see note below: your old constructor never set the Id
    }

    public override string ToString() => $"User: {Id}: {UserName}";
}