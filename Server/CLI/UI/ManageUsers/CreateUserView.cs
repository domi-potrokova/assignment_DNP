using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IUserRepository userRepository;
    
    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        Console.Write("Username: ");
        string? userName = Console.ReadLine();
        Console.Write("Password: ");
        string? password = Console.ReadLine();
        
        //Readline may return null, check before useing the values
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Username + password is required");
            return;
        }
        
        //userName must be unique
        //ordinalIgnoreCase makes "Domi" and "DOMI" the same
        bool taken = userRepository.GetMany()
            .Any(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

        if (taken)
        {
            Console.WriteLine("Username already exists");
            return; //stop before saving
        }

        User created = await userRepository.AddAsync(new User { UserName = userName, Password = password });
        Console.WriteLine($"User created with id: {created.Id}");
    }
}