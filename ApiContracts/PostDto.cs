namespace ApiContracts;

public class PostDto
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public required int UserId { get; set; }
    public required string UserName { get; set; }

    // Only filled when the client asks for comments to be included
    public List<CommentDto>? Comments { get; set; }
}
