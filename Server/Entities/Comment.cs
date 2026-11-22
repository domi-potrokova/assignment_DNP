// File: Entities/Comment.cs
namespace Entities;

public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; }

    // Two relationships: a comment belongs to one user and to one post
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    private Comment() { } // for EFC

    public Comment(int id, string body, int userId, int postId)
    {
        Id = id;
        Body = body;
        UserId = userId;
        PostId = postId;
    }

    public override string ToString() => $"Comment: {Id} (by User: {UserId} on Post #{PostId}):\n{Body}";
}