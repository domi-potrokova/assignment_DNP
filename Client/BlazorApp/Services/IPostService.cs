using ApiContracts;

namespace BlazorApp.Services;

public interface IPostService
{
    public Task<PostDto> CreatePostAsync(CreatePostDto request);
    public Task UpdatePostAsync(int id, UpdatePostDto request);
    public Task DeletePostAsync(int id);
    public Task<PostDto> GetSinglePostAsync(int id, bool includeComments =false);
    //giving option as null = not mandatory for user to fill it in, it will work without it
    public Task<IEnumerable<PostDto>> GetManyPostsAsync(string? title = null, int? userId = null, string? userName = null);
}