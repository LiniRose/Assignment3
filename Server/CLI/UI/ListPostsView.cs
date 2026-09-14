using RepositoryContracts;

namespace CLI.UI;

public class ListPostsView
{
    private readonly IPostRepository postRepository;

    public ListPostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public Task ShowAsync()
    {
        Console.WriteLine();
        Console.WriteLine("-Posts overview-");

        var posts = postRepository.GetManyAsync().ToList();

        if (!posts.Any())
        {
            Console.WriteLine("No posts found.");
            return Task.CompletedTask;
        }

        foreach (var post in posts)
        {
            Console.WriteLine($"[{post.Id}] {post.Title}");
        }

        return Task.CompletedTask;
    }
}