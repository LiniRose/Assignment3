using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser([FromBody] CreateUserDto request)
    {
        if (UserNameInUse(request.UserName, null))
        {
            return Conflict($"Username '{request.UserName}' is already taken.");
        }

        User user = new User
        {
            UserName = request.UserName,
            Password = request.Password
        };
        User created = await userRepo.AddAsync(user);

        UserDto dto = ToDto(created);
        return Created($"/users/{dto.Id}", dto);
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto request)
    {
        // Throws (-> 404 via middleware) if the user does not exist
        await userRepo.GetSingleAsync(id);

        if (UserNameInUse(request.UserName, id))
        {
            return Conflict($"Username '{request.UserName}' is already taken.");
        }

        User user = new User
        {
            Id = id,
            UserName = request.UserName,
            Password = request.Password
        };
        await userRepo.UpdateAsync(user);
        return NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser([FromRoute] int id)
    {
        User user = await userRepo.GetSingleAsync(id);
        return Ok(ToDto(user));
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetUsers([FromQuery] string? userName)
    {
        IEnumerable<User> users = userRepo.GetManyAsync();

        if (!string.IsNullOrWhiteSpace(userName))
        {
            users = users.Where(u => u.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(users.Select(ToDto).ToList());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        await userRepo.DeleteAsync(id);
        return NoContent();
    }

    private bool UserNameInUse(string userName, int? exceptUserId)
    {
        return userRepo.GetManyAsync().Any(u =>
            u.UserName.ToLower() == userName.ToLower() && u.Id != exceptUserId);
    }

    private static UserDto ToDto(User user) => new()
    {
        Id = user.Id,
        UserName = user.UserName
    };
}
