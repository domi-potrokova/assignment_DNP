using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class CreatePostView
{
    // Interfaces, not concrete classes: the real objects come from Program.cs
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public CreatePostView(IPostRepository postRepository, IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        Console.Write("Title: ");
        string? title = Console.ReadLine(); // string? because ReadLine can return null

        Console.Write("Body: ");
        string? body = Console.ReadLine();

        Console.Write("User id: ");
        string? userIdInput = Console.ReadLine();

        // Validate the text fields before using them
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Title and body are required.");
            return;
        }

        // TryParse returns false instead of crashing if the input isn't a number
        if (!int.TryParse(userIdInput, out int userId))
        {
            Console.WriteLine("User id must be a number.");
            return;
        }

        // Check that the user exists. GetMany().Any(...) returns true/false,
        // so it works no matter how GetSingle behaves when nothing is found
        if (!userRepository.GetMany().Any(u => u.Id == userId))
        {
            Console.WriteLine($"No user with id {userId}.");
            return;
        }

        // 0 is a placeholder: the repository assigns the real id inside Add()
        Post post = new Post(0, title, body, userId);
        Post created = await postRepository.AddAsync(post);

        Console.WriteLine($"Created post with id {created.Id}");
    }
}