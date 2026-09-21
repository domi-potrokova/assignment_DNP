using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ListPostsView
{
    private readonly IPostRepository postRepository;
    
    public ListPostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public Task ShowAsync()
    {
        //GetMany() returns IQueryable<post>, so filtering later is just .Where(...)
        foreach (var post in postRepository.GetMany())
        {
            Console.WriteLine($"[{post.Id}] {post.Title}");
        }
        return Task.CompletedTask;
    }
}