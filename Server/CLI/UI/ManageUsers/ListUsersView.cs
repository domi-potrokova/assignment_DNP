using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ListUsersView
{
    private readonly IUserRepository userRepository;
    
    public ListUsersView(IUserRepository userRepository)
    {
      this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        //GetMany() returns IQueryable<User>, so filtering later is just .Where(...)
        foreach (var user in userRepository.GetMany())
        {
            Console.WriteLine($"[{user.Id}] {user.UserName}");
        }
    }
}