using RepositoryContracts;

namespace CLI.UI.ManageUsers;

//creates CreateUserView + ListUsersView and shows small menu that calls one of them depending on what the user types
public class ManageUsersView
{
    //readonly protects from accidentally changing it
    private readonly CreateUserView createUserView;
    private readonly ListUsersView listUsersView;

    public ManageUsersView(IUserRepository userRepository)
    {
        //both views share the same repo, so they see the same data
        createUserView = new CreateUserView(userRepository);
        listUsersView = new ListUsersView(userRepository);
    }

    public async Task ShowAsync()
    {
        while (true) //submenu loop; returning goes back to CliApps menu
        {
            Console.WriteLine("\n-- Users --\n1) Create\n2) List all\n0) Back");
            string? choice = Console.ReadLine();

            if (choice == "1") await createUserView.ShowAsync();
            else if (choice == "2") await listUsersView.ShowAsync();
            else if (choice == "0") return;
            else Console.WriteLine("Invalid choice");
            
            //at the end we must link it to CliApp
        }
    }
}