using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

//singlePostView must have access to all repos, it finds the postId, relevants comments to it and checks if the user exists
public class SinglePostView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public SinglePostView(IPostRepository postRepository,
        ICommentRepository commentRepository,
        IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        Console.Write("Post id: ");
    string? input = Console.ReadLine(); // string? because ReadLine can return null

    // TryParse returns false instead of crashing when the input isn't a number
    if (!int.TryParse(input, out int postId))
    {
        Console.WriteLine("Post id must be a number.");
        return;
    }

    // FirstOrDefault returns null if nothing matches, so the type is Post?
    // (this avoids depending on how GetSingle behaves when the id is missing)
    Post? post = postRepository.GetMany().FirstOrDefault(p => p.Id == postId);
    if (post is null)
    {
        Console.WriteLine($"No post with id {postId}.");
        return;
    }

    // Print the post: ToString() in your Post class is used automatically
    Console.WriteLine();
    Console.WriteLine(post);

    // Only the comments that belong to this post (PostId is the foreign key)
    var comments = commentRepository.GetMany()
        .Where(c => c.PostId == postId)
        .ToList();

    Console.WriteLine("\nComments:");
    if (comments.Count == 0)
    {
        Console.WriteLine("(no comments yet)");
    }
    foreach (var comment in comments)
    {
        Console.WriteLine($"- [{comment.Id}] user {comment.UserId}: {comment.Body}");
    }

    // Offer to add a comment
    Console.Write("\nAdd a comment? (y/n): ");
    if (Console.ReadLine()?.ToLower() == "y") // ?. skips ToLower if ReadLine returned null
    {
        await AddCommentAsync(postId);
    }
}

// Separate method so Show() stays readable
    private async Task AddCommentAsync(int postId)
    {
        Console.Write("Comment: ");
        string? body = Console.ReadLine();

        Console.Write("Your user id: ");
        string? userIdInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Comment can't be empty.");
            return;
        }

        if (!int.TryParse(userIdInput, out int userId))
        {
            Console.WriteLine("User id must be a number.");
            return;
        }

        // The user must exist before they can comment
        if (!userRepository.GetMany().Any(u => u.Id == userId))
        {
            Console.WriteLine($"No user with id {userId}.");
            return;
        }

        // 0 is a placeholder id: the repository assigns the real one in Add().
        // Check your Comment constructor and match its parameter order.
        Comment comment = new Comment(0, body, postId, userId);
        await commentRepository.AddAsync(comment);
        Console.WriteLine("Comment added.");
    }
}