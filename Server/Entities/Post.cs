namespace Entities;
//we use auto-properties instead of field variables
public class Post
{
    //property, backed by method (getter/setter) = C#'s way of controling access to
    //an obj data while still using simple obj.PropertyName syntax
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; } //FK only not navigation property
    
    //public User UserId { get; set; } = can't use bc requirements say we use FK rather than associations
    //public SubForum SubForumId { get; set; } //navigation property = NOT IN THIS PROJECT, WE USE FKs

    public Post(int id, string title, string body, int userId)
    {
        Id = id;
        Title = title;
        Body = body;
        UserId = userId;
    }

    public override string ToString()
    {
        return $"Post: {Id}: \"{Title}\" (by User: {UserId})\n{Body}";
    }
    
    // $ = interpolated string, C# evaluates them and inserts the result as text
    // \ = escape sequence = so that C# is not mistaking it for closing quote
}
