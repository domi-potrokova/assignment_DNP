using Entities;

namespace RepositoryContracts;

public interface IPostRepository
{
    Post Add(Post post); //returns created post
    void Update(Post post); //replaces existing post
    void Delete(int id); //removes post
    Post GetSingle(int id); //returns post matching the given ID
    IQueryable<Post> GetMany(); //loops over to extract the relevant entities
}