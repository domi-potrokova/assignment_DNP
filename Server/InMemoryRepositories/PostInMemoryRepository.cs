using RepositoryContracts;
using Entities;

namespace InMemoryRepositories;

//CRUD logic = all classes have the same logic
//optimization = not copy pasting, but making a generic base class as abstract and implementing logic to each of concrete classes
public class PostInMemoryRepository : IPostRepository
{
    private readonly List<Post> posts = new();

    public PostInMemoryRepository()
    {
        SeedData(); //runs once whenprogram.cs crates the repos
    }

    private void SeedData()
    {
        posts.Add(new Post(1, "Welcome", "First post on the forum", 1));
        posts.Add(new Post(2, "C# tips", "Use interfaces for repositories", 2));
        posts.Add(new Post(3, "Homework help", "Anyone done assignment 2?", 3));
    }

    //takes a new post that doesn't have ID yet, assigns one automatically and stores in memory of list
    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any() //any() = checks posts list if there is at least one T/F
            ? posts.Max(p => p.Id) + 1 //if T list has already posts, used lambda ex. for each p,give me its Id, Max()
            : 1; //if F just use 1 as a starting number
        posts.Add(post);
        return Task.FromResult(post); //wraps a ready value in a Task
    }

    public Task UpdateAsync(Post post)
    {
        //? = marked as nullable ref type(variable is allowed null)
        //SingleOrDefault = searches the posts list for the one post whose ID matches the incoming post.id
        //single = thr exception
        //singleOrDefault = returns null if nothing matches
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException($"Post with id {post.Id} does not exist");
        }
        posts.Remove(existingPost);
        posts.Add(post);
        return Task.CompletedTask; //task with no result
    }

    public Task DeleteAsync(int id)
    {
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException($"Post with id {id} does not exist");
        }
        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException($"Post with id {id} not found");
        }
        return Task.FromResult(post);
    }

    public IQueryable<Post> GetMany()
    {
        return posts.AsQueryable();
    }
}