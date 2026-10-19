namespace ApiContracts;

public class CreateCommentDto
{
    public required string Body { get; set; }
    public required int UserId { get; set; }
    public required int PostId { get; set; } //needed bc comments are flat, not nested under /posts
}