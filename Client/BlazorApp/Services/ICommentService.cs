using ApiContracts;

namespace BlazorApp.Services;

public interface ICommentService
{
    public Task<CommentDto> CreateCommentAsync(CreateCommentDto request);
    public Task UpdateCommentAsync(int id, UpdateCommentDto request);
    public Task DeleteCommentAsync(int id);
    public Task<CommentDto> GetSingleCommentAsync(int id);
    public Task<IEnumerable<CommentDto>> GetManyCommentsAsync(int? postId = null, int? userId = null,
        string? userName = null);
}