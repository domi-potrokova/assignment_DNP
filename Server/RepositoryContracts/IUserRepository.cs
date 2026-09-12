using Entities;

namespace RepositoryContracts;

public interface IUserRepository
{
    User Add(User user);
    void Update(User user);
    void Delete(int id);
    User GetSingle(int id);
    IQueryable<User> GetMany();
}