using RepositoryContracts;

namespace CLI.UI.ManagePosts;

//creates CreatePostView + List + Single
public class ManagePostsView
{
    private readonly CreatePostView createPostView;
    private readonly ListPostsView listPostsView;
    private readonly SinglePostView singlePostView;
    
    public  ManagePostsView(IPostRepository postRepository, IUserRepository userRepository, ICommentRepository commentRepository)
    {
        //eachworker gets only the repos it actually needs
      createPostView = new CreatePostView(postRepository, userRepository);
      listPostsView = new ListPostsView(postRepository);
      singlePostView = new SinglePostView(postRepository, commentRepository, userRepository);
    }

    public async Task ShowAsync()
    {
        while (true)
        {
            Console.WriteLine("\n-- Posts --\n1) Create\n2) List all\n3) View single post\n0) Back");
            string? choice = Console.ReadLine();

            if (choice == "1") await createPostView.ShowAsync();
            else if (choice == "2") await listPostsView.ShowAsync();
            else if (choice == "3") await singlePostView.ShowAsync();
            else if (choice == "0") return;
            else Console.WriteLine("Unknown option");
            
            //at the end we must link managePostView to CliApp 
        }
    }
}