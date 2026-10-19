using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;
    private readonly ICommentRepository commentRepo;

    public PostsController(IPostRepository postRepo, IUserRepository userRepo, ICommentRepository commentRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
        this.commentRepo = commentRepo;
    }
    
    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost([FromBody] CreatePostDto request)
    {
        if (!userRepo.GetManyAsync().Any(u => u.Id == request.UserId))
        {
            return BadRequest($"User with ID '{request.UserId}' does not exist.");
        }

        Post post = new Post
        {
            Title = request.Title,
            Body = request.Body,
            UserId = request.UserId
        };
        Post created = await postRepo.AddAsync(post);

        return Created($"/posts/{created.Id}", ToDto(created, UserNames()));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePost([FromRoute] int id, [FromBody] UpdatePostDto request)
    {
        Post existing = await postRepo.GetSingleAsync(id);

        Post post = new Post
        {
            Id = id,
            Title = request.Title,
            Body = request.Body,
            UserId = existing.UserId
        };
        await postRepo.UpdateAsync(post);
        return NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetPost([FromRoute] int id, [FromQuery] bool includeComments = false)
    {
        Post post = await postRepo.GetSingleAsync(id);
        Dictionary<int, string> names = UserNames();

        PostDto dto = ToDto(post, names);
        if (includeComments)
        {
            dto.Comments = CommentsForPost(id, names);
        }

        return Ok(dto);
    }

    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetPosts(
        [FromQuery] string? titleContains,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        Dictionary<int, string> names = UserNames();
        IEnumerable<Post> posts = postRepo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(titleContains))
        {
            posts = posts.Where(p => p.Title.Contains(titleContains, StringComparison.OrdinalIgnoreCase));
        }

        if (userId is not null)
        {
            posts = posts.Where(p => p.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            HashSet<int> matchingIds = names
                .Where(n => n.Value.Contains(userName, StringComparison.OrdinalIgnoreCase))
                .Select(n => n.Key)
                .ToHashSet();
            posts = posts.Where(p => matchingIds.Contains(p.UserId));
        }

        return Ok(posts.Select(p => ToDto(p, names)).ToList());
    }

    [HttpGet("{postId:int}/comments")]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsForPost([FromRoute] int postId)
    {
        await postRepo.GetSingleAsync(postId); // 404 if the post doesn't exist
        return Ok(CommentsForPost(postId, UserNames()));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost([FromRoute] int id)
    {
        await postRepo.DeleteAsync(id);

        List<Comment> orphaned = commentRepo.GetManyAsync().Where(c => c.PostId == id).ToList();
        foreach (Comment comment in orphaned)
        {
            await commentRepo.DeleteAsync(comment.Id);
        }

        return NoContent();
    }

    private Dictionary<int, string> UserNames()
    {
        return userRepo.GetManyAsync().ToDictionary(u => u.Id, u => u.UserName);
    }

    private List<CommentDto> CommentsForPost(int postId, Dictionary<int, string> names)
    {
        return commentRepo.GetManyAsync()
            .Where(c => c.PostId == postId)
            .ToList()
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Body = c.Body,
                UserId = c.UserId,
                UserName = names.GetValueOrDefault(c.UserId, "Unknown"),
                PostId = c.PostId
            })
            .ToList();
    }

    private static PostDto ToDto(Post post, Dictionary<int, string> names) => new()
    {
        Id = post.Id,
        Title = post.Title,
        Body = post.Body,
        UserId = post.UserId,
        UserName = names.GetValueOrDefault(post.UserId, "Unknown")
    };
}
