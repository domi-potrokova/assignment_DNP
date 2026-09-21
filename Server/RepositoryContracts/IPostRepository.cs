using Entities;

namespace RepositoryContracts;

public interface IPostRepository
{
    Task<Post> AddAsync(Post post); //returns created post
    Task UpdateAsync(Post post); //replaces existing post
    Task DeleteAsync(int id); //removes post
    Task<Post> GetSingleAsync(int id); //returns post matching the given ID
    IQueryable<Post> GetMany(); //loops over to extract the relevant entities
}