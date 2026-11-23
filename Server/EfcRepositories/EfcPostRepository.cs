using System.Globalization;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RepositoryContracts;

namespace EfcRepositories;

public class EfcPostRepository : IPostRepository
{
    //each repo needs access to DbContext, so i inject that through constructor
    private readonly AppContext ctx;

    public EfcPostRepository(AppContext ctx)
    {
        this.ctx = ctx;
    }
    
    //Add
    public async Task<Post> AddAsync(Post post)
    {
        EntityEntry<Post> entityEntry = await ctx.Posts.AddAsync(post);
        await ctx.SaveChangesAsync();
        return entityEntry.Entity; //the final post = post gets an id (contained in the entityEntry.Entity)
    }
    
    //Update = first check if the post exists and than update method
    public async Task UpdateAsync(Post post)
    {
        if (!(await ctx.Posts.AnyAsync(p => p.Id == post.Id)))
        {
            throw new KeyNotFoundException($"Post with id {post.Id} not found");
        }

        ctx.Posts.Update(post);
        await ctx.SaveChangesAsync();
    }
    
    //Delete = in order to delete an entity, it must be fetched and than removed
    public async Task DeleteAsync(int id)
    {
        Post? existing = await ctx.Posts.SingleOrDefaultAsync(p => p.Id == id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Post with id {id} not found");
        }

        ctx.Posts.Remove(existing);
        await ctx.SaveChangesAsync();
    }
    
    //GetSingle = fetch one post by id, or throw it doens't exist
    public async Task<Post> GetSingleAsync(int id)
    {
        //SingleOrDefaultAsync returns null when nothing matches, instead of throwing
        Post? post = await ctx.Posts.SingleOrDefaultAsync(p => p.Id == id);
        
        if (post == null)
        {
            throw new KeyNotFoundException($"Post with id {id} not found");
        }

        return post;
    }
    
    //GetMany
    public IQueryable<Post> GetMany()
    {
        return ctx.Posts.AsQueryable();
    }
}