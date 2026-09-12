using RepositoryContracts;
using Entities;

namespace InMemoryRepositories;

//CRUD logic = all classes have the same logic
//optimization = not copy pasting, but making a generic base class as abstract and implementing logic to each of concrete classes
public class PostInMemoryRepository : IPostRepository
{
     List<Post> posts = new();

    //takes a new post that doesn't have ID yet, assigns one automatically and stores in memory of list
    public Post Add(Post post)
    {
        post.Id = posts.Any() //any() = checks posts list if there is at least one T/F
            ? posts.Max(p => p.Id) + 1 //if T list has already posts, used lambda ex. for each p,give me its Id, Max()
            : 1; //if F just use 1 as a starting number
        posts.Add(post);
        return post;
    }

    public void Update(Post post)
    {
        //? = marked as nullable ref type(variable is allowed null)
        //SingleOrDefault = searches the posts list for the one post whose ID matches the incoming post.id
        //single = thr exception
        //singleOrDefault = returns nullif nothing matches
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException(
                $"Post with id {post.Id} does not exist");
        }
        posts.Remove(existingPost);
        posts.Add(post);
    }

    public void Delete(int id)
    {
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException(
                $"Post with id {id} does not exist");
        }
        posts.Remove(postToRemove);
    }

    public Post GetSingle(int id)
    {
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException(
                $"Post with id {id} not found");
        }
        return post;
    }

    public IQueryable<Post> GetMany()
    {
        return posts.AsQueryable();
    }
}