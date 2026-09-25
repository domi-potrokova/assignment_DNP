using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

//not a private list, but list is represented by file
//each method must read JSON from the file + deserialize JSON to list
//interact with the list (add + retrieve + delete + overwrite)
//serialize the list to JSON
//write the JSON back to the file, overwrite the existing content

public class CommentFileRepository : ICommentRepository
{
    //creating file path = file per entity
    private readonly string filePath = "comments.json";
    
    //constructor ensures that there is a file, if none exists (first time) a new
    //file is created with the content of empty list []
    public CommentFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        //read all the content from the file, file is in JSON format
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        
        //JSON is deserialized into a list of comments = ! is a warning that the value in list can be null
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        
        //calculating the next ID to use
        //c is parameter and put together with comments.Max the compiler knows that c is single comment
        int maxId = comments.Count > 0 ? comments.Max(c => c.Id) : 0;
        
        //set the ID
        comment.Id = maxId + 1;
        
        //add the comment to the list
        comments.Add(comment);
        
        //serialize the list to JSON
        commentsAsJson = JsonSerializer.Serialize(comments);
        
        //write the JSON back to file
        await File.WriteAllTextAsync(filePath, commentsAsJson);
        
        //return the finalized comment now that it has ID
        return comment;
    }

    //it takes int Id, from RepositoryContracts
    //all i need to know is Id to delete nothing else
    //DeleteAsync needs to load the list + find the comment whose Id matches the one i want to remove + save it
    public async Task DeleteAsync(int id)
    {
        string commentAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentAsJson)!;
        
        //find the one comment with this id
        //same operation as in CommentInMemoryRepository
       Comment? commentToRemove = comments.SingleOrDefault(c => c.Id == id);
       if (commentToRemove is null)
       {
           throw new InvalidOperationException($"Comment with id {id} not found");
       }
       
        comments.Remove(commentToRemove);
        commentAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentAsJson);
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        string commentAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentAsJson)!;

        Comment? comment = comments.SingleOrDefault(c => c.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException(
                $"Comment with id {id} not found");
        }
        return comment;
    }
    
    public async Task UpdateAsync(Comment comment)
    {
        string commentAsJson = await File.ReadAllTextAsync(filePath);
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentAsJson)!;
        
        Comment? existingComment = comments.SingleOrDefault(c => c.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException(
                $"Comment with id {comment.Id} does not exist");
        }
        comments.Remove(existingComment);
        comments.Add(comment);
       
        commentAsJson = JsonSerializer.Serialize(comments);
        await File.WriteAllTextAsync(filePath, commentAsJson);
    }
    
    //getMany is sync, so no await, async no Task<..>
    //asking for queries, request for data with some condition 
    public IQueryable<Comment> GetMany()
    {
        string commentsAsJson = File.ReadAllText(filePath); //sync read, matches the interface
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentsAsJson)!;
        return comments.AsQueryable();
    }
}