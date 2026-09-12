using RepositoryContracts;
using Entities;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    List<Comment> comments = new();

    public Comment Add(Comment comment)
    {
        comment.Id = comments.Any()
            ? comments.Max(c => c.Id) + 1
            : 1;
        comments.Add(comment);
        return comment;
    }

    public void Update(Comment comment)
    {
        Comment? existingComment = comments.SingleOrDefault(c => c.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException(
                $"Comment with id {comment.Id} does not exist");
        }
        comments.Remove(existingComment);
        comments.Add(comment);
    }

    public void Delete(int id)
    {
        Comment? commentToRemove = comments.SingleOrDefault(c => c.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException(
                $"Comment with id {id} does not exist");
        }
        comments.Remove(commentToRemove);
    }

    public Comment GetSingle(int id)
    {
        Comment? comment = comments.SingleOrDefault(c => c.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException(
                $"Comment with id {id} not found");
        }
        return comment;
    }

    public IQueryable<Comment> GetMany()
    {
        return comments.AsQueryable();
    }
    }