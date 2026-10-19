namespace ApiContracts;

//user Id is just number, we probably want also username so we add it to dto
//when looking at the post do we want to see the comments too? making a query parameter
//used for GetMany (list overview) and as the base shape for getSingle
public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }
    public string? AuthorUserName { get; set; } //filled in by the controller, not org stored in post itself
    
    //query comment: null when comments are not requested, populated when they are
    public List<CommentDto>? Comments { get; set; }
}