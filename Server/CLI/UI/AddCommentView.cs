using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class AddCommentView
{
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public AddCommentView(ICommentRepository commentRepository, IPostRepository postRepository, IUserRepository userRepository)
    {
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }
    
    public async Task ShowAsync()
    {
        Console.WriteLine();
        Console.WriteLine("-Add comment to a post-");

        Console.Write("Post ID: ");
        string? postIdInput = Console.ReadLine();

        Console.Write("User ID: ");
        string? userIdInput = Console.ReadLine();

        Console.Write("Comment: ");
        string? body = Console.ReadLine();

        if (!int.TryParse(postIdInput, out int postId) || !int.TryParse(userIdInput, out int userId))
        {
            Console.WriteLine("Invalid ID. Comment not added.");
            return;
        }

        try
        {
            await postRepository.GetSingleAsync(postId);
            await userRepository.GetSingleAsync(userId);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Could not add comment: {ex.Message}");
            return;
        }

        Comment comment = new Comment
        {
            Body = body ?? string.Empty,
            UserId = userId,
            PostId = postId
        };

        Comment created = await commentRepository.AddAsync(comment);

        Console.WriteLine($"Comment added with ID {created.Id}.");
    }
}