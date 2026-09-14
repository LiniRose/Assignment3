using RepositoryContracts;

namespace CLI.UI;

public class ViewPostView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public ViewPostView(IPostRepository postRepository, ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }
    
    public async Task ShowAsync()
    {
        Console.WriteLine();
        Console.WriteLine("-View a specific post-");

        Console.Write("Post ID: ");
        string? postIdInput = Console.ReadLine();

        if (!int.TryParse(postIdInput, out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }

        try
        {
            var post = await postRepository.GetSingleAsync(postId);

            Console.WriteLine();
            Console.WriteLine($"Title: {post.Title}");
            Console.WriteLine($"Body: {post.Body}");
            Console.WriteLine();
            Console.WriteLine("Comments:");

            var comments = commentRepository.GetManyAsync()
                .Where(c => c.PostId == postId)
                .ToList();

            if (!comments.Any())
            {
                Console.WriteLine("  (no comments yet)");
            }
            else
            {
                foreach (var comment in comments)
                {
                    Console.WriteLine($"  - {comment.Body} (by user {comment.UserId})");
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}