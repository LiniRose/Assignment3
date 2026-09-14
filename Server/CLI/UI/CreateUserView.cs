using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class CreateUserView
{
    private readonly IUserRepository userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    
    public async Task ShowAsync()
    {
        Console.WriteLine();
        Console.WriteLine("-Create new user-");

        Console.Write("Username: ");
        string? userName = Console.ReadLine();

        Console.Write("Password: ");
        string? password = Console.ReadLine();

        User user = new User
        {
            UserName = userName ?? string.Empty,
            Password = password ?? string.Empty
        };

        User created = await userRepository.AddAsync(user);

        Console.WriteLine($"User '{created.UserName}' created with ID {created.Id}.");
    }
    
}