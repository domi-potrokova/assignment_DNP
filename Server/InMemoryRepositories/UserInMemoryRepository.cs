using RepositoryContracts;
using Entities;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    List<User> users = new();

    public User Add(User user)
    {
        user.Id = users.Any()
            ? users.Max(u => u.Id) + 1
            : 1;
        users.Add(user);
        return user;
    }

    public void Update(User user)
    {
        User? existingUser = users.SingleOrDefault(u => u.Id == user.Id);
        if (existingUser is null)
        {
            throw new InvalidOperationException(
                $"User with id {user.Id} does not exist");
        }
        users.Remove(existingUser);
        users.Add(user);
    }

    public void Delete(int id)
    {
        User? userToRemove = users.SingleOrDefault(u => u.Id == id);
        if (userToRemove is null)
        {
            throw new InvalidOperationException(
                $"User with id {id} does not exist");
        }
        users.Remove(userToRemove);
    }

    public User GetSingle(int id)
    {
        User? user = users.SingleOrDefault(u => u.Id == id);
        if (user is null)
        {
            throw new InvalidOperationException(
                $"User with id {id} not found");
        }
        return user;
    }

    public IQueryable<User> GetMany()
    {
        return users.AsQueryable();
    }
}