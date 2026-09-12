using Entities;

namespace RepositoryContracts;

public interface ICommentRepository
{
    Comment Add(Comment comment);
    void Update(Comment comment);
    void Delete(int id);
    Comment GetSingle(int id);
    IQueryable<Comment> GetMany();
}