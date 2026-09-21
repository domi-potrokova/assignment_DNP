using CLI.UI.ManagePosts;
using CLI.UI.ManageUsers;
using RepositoryContracts;

namespace CLI.UI;

//CliApp creates ManageUsersView and pases it the repos it needs
//CliApp never touches CreateUserView directly
//after program.cs passed everything to CliApp, the app is started and call is AWAITED
//async method can be called only by other async methods.
public class CliApp
{
   private readonly ManageUsersView manageUsersView;
   private readonly ManagePostsView managePostsView;
   public CliApp(IUserRepository userRepository, ICommentRepository commentRepository, IPostRepository postRepository)
   {
      //CliApp creates the sub-view and passes on only what it needs
      manageUsersView = new ManageUsersView(userRepository);
      managePostsView = new ManagePostsView(postRepository, userRepository, commentRepository);
   }

   public async Task StartAsync()
   {
      while (true) //main menu loop, runs until the user picks 0
      {
         Console.WriteLine("\n1) Manage users\n2) Manage posts\n0) Exit"); //need to update this after ManagePostsView is completed
         string? choice = Console.ReadLine(); //string? because readLine can return null

         if (choice == "1") await manageUsersView.ShowAsync();
         else if (choice == "2") await managePostsView.ShowAsync();  
         else if (choice == "0") return;
         else Console.WriteLine("Invalid choice");
      }
   }
}