using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("posts")]
public class PostController : ControllerBase //controllerBase = base class every API controller inherits from (gives me Ok.. NotFound...)
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository; //to look up the authors username for postDto
    private readonly ICommentRepository commentRepository; //to optionally include comments

    public PostController(IPostRepository postRepository, IUserRepository userRepository,
        ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
    }
    
    //POST/posts
    [HttpPost]
    //Task<> = this is an async method, the real result will be ready later, not immediately
    //actionResult = lets method return either success or error response 
    public async Task<ActionResult<PostDto>> Create(CreatePostDto dto)
    {
        //checking if the user id in the request actually exists
        //get many checks in the list if the given user exists
        bool userExists = userRepository.GetMany().Any(u => u.Id == dto.UserId);
        if (!userExists)
        {
            return BadRequest($"No user with id {dto.UserId}."); //400
        }

        Post post = new Post(0, dto.Title, dto.Body, dto.UserId);
        Post created = await postRepository.AddAsync(post);
        
        //AuthorUserName isn't a field on Post itself, to get actual username, i need to fetch that user first
        //fetch = go ask repo for a specific obj by its id
        User author = await userRepository.GetSingleAsync(created.UserId);

        PostDto resultDto = new PostDto
        {
            Id = created.Id,
            Title = created.Title,
            Body = created.Body,
            UserId = created.UserId,
            AuthorUserName = author.UserName,
            Comments = null
        };
        //CreatedAtAction = status code (201 created) + header (location = method name, how to find it) + JSON body (generated post id + authors name)
        return CreatedAtAction(nameof(GetSingle), new { id = resultDto.Id }, resultDto);
    }
    
    //PUT/posts/{id}
    //only Task<ActionResult> no <PostDto> bc im not returning anything, i just send the status code with empty body
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdatePostDto dto)
    {
        //need to think which fields dto is allowed to change
        //i want to update only title + body, but constructor of Post has userId. Dto doesn't carry it, the only place to get the correct value is
        //from existing post already saved in repo (thats why i have to fetch it first and not map)
        
        //IMPORTANT: if the DTO contains every field the entity's constructor needs = build it directly from DTO
                    //if the DTO is missing a field on purpose (i dont want it updatable) = must fetch the existing entity first to recover the missing value
        
        Post existingPost;
        try
        {
            existingPost = await postRepository.GetSingleAsync(id); //bc of the missing userId, fetching from repo
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Post with id {id} not found");
        }
        //keep the original userId, only title and body change
        Post post = new Post(id, dto.Title, dto.Body, existingPost.UserId);
        await postRepository.UpdateAsync(post);

        return NoContent(); //204 success
    }
    
    //GET/posts/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetSingle(int id, [FromQuery] bool includeComments = false)
    {
        Post post;
        try
        {
            post = await postRepository.GetSingleAsync(id);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Post with id {id} not found");
        }

        User author = await userRepository.GetSingleAsync(post.UserId); //look up the author
        
        //copying the fields the client should see into PostDto, including the extra AuthorUserName i looked up
        //only reshaping the data how client is supposed to receive it
        PostDto dto = new PostDto()
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId,
            AuthorUserName = author.UserName,
            Comments = null
        };
        if (includeComments)
        {
            dto.Comments = commentRepository.GetMany()
                .Where(c => c.PostId == post.Id)
                .Select(c => new CommentDto()
                {
                    Id = c.Id,
                    Body = c.Body,
                    UserId = c.UserId,
                    PostId = c.PostId
                })
                .ToList();
        }

        return Ok(dto);
    }
    
    //GET/posts?userId
    //adding filters: it must be possible to filter by title containing certain string, post written by certain user
    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetMany(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        IQueryable<Post> posts = postRepository.GetMany();
        
        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }
        if (userId.HasValue)
        {
            posts = posts.Where(p => p.UserId == userId.Value);
        }
        if (!string.IsNullOrWhiteSpace(userName))
        {
            // Post only stores UserId, so filtering by author name means
            // first finding which user ids match that name, then filtering posts by those ids.
            List<int> matchingUserIds = userRepository.GetMany()
                .Where(u => u.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToList();
            posts = posts.Where(p => matchingUserIds.Contains(p.UserId));
        }
        
        //map to DTO without comments or author lookups, to keep the list endpoint fast
        //author username/comments stay available on GetSingle for a full view
        List<PostDto> dtos = posts
            .Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                UserId = p.UserId,
                AuthorUserName = null,
                Comments = null
            })
            .ToList();

        return Ok(dtos);
    } 
    
    //DELETE/posts/{id}
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await postRepository.DeleteAsync(id);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Post with id {id} not found");
        }

        return NoContent();
    }
}
