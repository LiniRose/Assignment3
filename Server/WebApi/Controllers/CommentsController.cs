using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public CommentsController(ICommentRepository commentRepo, IPostRepository postRepo, IUserRepository userRepo)
    {
        this.commentRepo = commentRepo;
        this.postRepo = postRepo;
        this.userRepo = userRepo;
    }

    // POST /comments
    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment([FromBody] CreateCommentDto request)
    {
        if (!postRepo.GetManyAsync().Any(p => p.Id == request.PostId))
        {
            return BadRequest($"Post with ID '{request.PostId}' does not exist.");
        }

        if (!userRepo.GetManyAsync().Any(u => u.Id == request.UserId))
        {
            return BadRequest($"User with ID '{request.UserId}' does not exist.");
        }

        Comment comment = new Comment
        {
            Body = request.Body,
            UserId = request.UserId,
            PostId = request.PostId
        };
        Comment created = await commentRepo.AddAsync(comment);

        return Created($"/comments/{created.Id}", ToDto(created, UserNames()));
    }

    // PUT /comments/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentDto request)
    {
        Comment existing = await commentRepo.GetSingleAsync(id);

        Comment comment = new Comment
        {
            Id = id,
            Body = request.Body,
            UserId = existing.UserId,
            PostId = existing.PostId
        };
        await commentRepo.UpdateAsync(comment);
        return NoContent();
    }

    // GET /comments/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetComment([FromRoute] int id)
    {
        Comment comment = await commentRepo.GetSingleAsync(id);
        return Ok(ToDto(comment, UserNames()));
    }

    // GET /comments?postId=1&userId=2&userName=ali
    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetComments(
        [FromQuery] int? postId,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        Dictionary<int, string> names = UserNames();
        IEnumerable<Comment> comments = commentRepo.GetManyAsync();

        if (postId is not null)
        {
            comments = comments.Where(c => c.PostId == postId);
        }

        if (userId is not null)
        {
            comments = comments.Where(c => c.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(userName))
        {
            HashSet<int> matchingIds = names
                .Where(n => n.Value.Contains(userName, StringComparison.OrdinalIgnoreCase))
                .Select(n => n.Key)
                .ToHashSet();
            comments = comments.Where(c => matchingIds.Contains(c.UserId));
        }

        return Ok(comments.Select(c => ToDto(c, names)).ToList());
    }

    // DELETE /comments/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int id)
    {
        await commentRepo.DeleteAsync(id);
        return NoContent();
    }

    private Dictionary<int, string> UserNames()
    {
        return userRepo.GetManyAsync().ToDictionary(u => u.Id, u => u.UserName);
    }

    private static CommentDto ToDto(Comment comment, Dictionary<int, string> names) => new()
    {
        Id = comment.Id,
        Body = comment.Body,
        UserId = comment.UserId,
        UserName = names.GetValueOrDefault(comment.UserId, "Unknown"),
        PostId = comment.PostId
    };
}
