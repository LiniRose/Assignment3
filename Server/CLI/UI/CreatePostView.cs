using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class CreatePostView
{
    private readonly IPostRepository postRepository;

    public CreatePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }
    
    public async Task ShowAsync()
    {
        Console.WriteLine();
        Console.WriteLine("-Create new post-");

        Console.Write("Title: ");
        string? title = Console.ReadLine();

        Console.Write("Body: ");
        string? body = Console.ReadLine();

        Console.Write("User ID: ");
        string? userIdInput = Console.ReadLine();

        if (!int.TryParse(userIdInput, out int userId))
        {
            Console.WriteLine("Invalid user ID. Post not created.");
            return;
        }

        Post post = new Post
        {
            Title = title ?? string.Empty,
            Body = body ?? string.Empty,
            UserId = userId
        };

        Post created = await postRepository.AddAsync(post);

        Console.WriteLine($"Post '{created.Title}' created with ID {created.Id}.");
    }
}