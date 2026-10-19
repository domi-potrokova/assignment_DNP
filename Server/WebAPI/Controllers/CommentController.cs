using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("comments")]
public class CommentController : ControllerBase
{
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;

    public CommentController(ICommentRepository commentRepository, IUserRepository userRepository)
    {
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
    }
    
    //POST/comments
    [HttpPost]
    public async Task<ActionResult<CommentDto>> Create(CreateCommentDto dto)
    {
        Comment comment = new Comment(0, dto.Body, dto.UserId, dto.PostId);
        Comment created = await commentRepository.AddAsync(comment);

        CommentDto resultDto = new CommentDto()
        {
            Id = created.Id,
            Body = created.Body,
            UserId = created.UserId,
            PostId = created.PostId
        };
        return CreatedAtAction(nameof(GetSingle), new { id = resultDto.Id }, resultDto);
    }
    
    // PUT /comments/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateCommentDto dto)
    {
        Comment existingComment;
        try
        {
            existingComment = await commentRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment with id {id} not found.");
        }

        // only Body is updatable — keep the original UserId and PostId
        Comment comment = new Comment(id, dto.Body, existingComment.UserId, existingComment.PostId);
        await commentRepository.UpdateAsync(comment);

        return NoContent();
    }
    
    // GET /comments/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetSingle(int id)
    {
        Comment comment;
        try
        {
            comment = await commentRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment with id {id} not found.");
        }

        CommentDto dto = new CommentDto
        {
            Id = comment.Id,
            Body = comment.Body,
            UserId = comment.UserId,
            PostId = comment.PostId
        };

        return Ok(dto);
    }
    
    
    // GET /comments?postId=1
    //adding filter: must be possible to filter by user id/name, post id
    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetMany(
        [FromQuery] int? postId,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        IQueryable<Comment> comments = commentRepository.GetMany();

        if (postId.HasValue)
        {
            comments = comments.Where(c => c.PostId == postId.Value);
        }

        if (userId.HasValue)
        {
            comments = comments.Where(c => c.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            List<int> matchingUserIds = userRepository.GetMany()
                .Where(u => u.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToList();

            comments = comments.Where(c => matchingUserIds.Contains(c.UserId));
        }
        
        List<CommentDto> dtos = comments
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Body = c.Body,
                UserId = c.UserId,
                PostId = c.PostId
            })
            .ToList();

        return Ok(dtos);
    }
    
    // DELETE /comments/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await commentRepository.DeleteAsync(id);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Comment with id {id} not found.");
        }

        return NoContent();
    }
}