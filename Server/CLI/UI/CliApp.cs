using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;
    private readonly IPostRepository postRepository;

    public CliApp(IUserRepository userRepository, ICommentRepository commentRepository, IPostRepository postRepository)
    {
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
        this.postRepository = postRepository;
    }

    public async Task StartAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("App running...");
            Console.WriteLine("1) Create new user");
            Console.WriteLine("2) Create new post");
            Console.WriteLine("3) Add comment to a post");
            Console.WriteLine("4) View posts overview");
            Console.WriteLine("5) View a specific post");
            Console.WriteLine("0) Exit");
            Console.Write("Option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await new CreateUserView(userRepository).ShowAsync();
                    break;
                case "2":
                    await new CreatePostView(postRepository).ShowAsync();
                    break;
                case "3":
                    await new AddCommentView(commentRepository, postRepository, userRepository).ShowAsync();
                    break;
                case "4":
                    await new ListPostsView(postRepository).ShowAsync();
                    break;
                case "5":
                    await new ViewPostView(postRepository, commentRepository).ShowAsync();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option, please try again.");
                    break;
            }
        }

        Console.WriteLine("Goodbye!");
    }
}