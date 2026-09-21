using RepositoryContracts;
using Entities;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
   private readonly List<Comment> comments = new();

   public CommentInMemoryRepository()
   {
       SeedData();
   }

   private void SeedData()
   {
       comments.Add(new Comment(1, "We are happy to have you", 1, 1));
       comments.Add(new Comment(2, "Wooooow", 2, 2));
       comments.Add(new Comment(3, "I don't want to", 1, 3));
   }

    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = comments.Any()
            ? comments.Max(c => c.Id) + 1
            : 1;
        comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        Comment? existingComment = comments.SingleOrDefault(c => c.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException(
                $"Comment with id {comment.Id} does not exist");
        }
        comments.Remove(existingComment);
        comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Comment? commentToRemove = comments.SingleOrDefault(c => c.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException(
                $"Comment with id {id} does not exist");
        }
        comments.Remove(commentToRemove);
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        Comment? comment = comments.SingleOrDefault(c => c.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException(
                $"Comment with id {id} not found");
        }
        return Task.FromResult(comment);
    }

    public IQueryable<Comment> GetMany()
    {
        return comments.AsQueryable();
    }
    }