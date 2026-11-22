// File: Entities/Post.cs
namespace Entities;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }

    // Foreign key + navigation property. The names "UserId" and "User" match,
    // so EFC links them automatically, no extra configuration needed.
    public int UserId { get; set; }
    public User User { get; set; } = null!; // null! = "EFC fills this in, never null when loaded"

    // A post has many comments
    public List<Comment> Comments { get; set; } = new();

    private Post() { } // for EFC

    public Post(int id, string title, string body, int userId)
    {
        Id = id;
        Title = title;
        Body = body;
        UserId = userId;
    }

    public override string ToString() => $"Post: {Id}: \"{Title}\" (by User: {UserId})\n{Body}";
}