using ApiContracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public AuthController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    // POST /auth/login
    [HttpPost("login")]
    public ActionResult<UserDto> Login([FromBody] LoginRequest request)
    {
        User? user = userRepo.GetManyAsync()
            .FirstOrDefault(u => u.UserName.ToLower() == request.UserName.ToLower());

        // Same message for "no such user" and "wrong password",
        if (user is null || user.Password != request.Password)
        {
            return Unauthorized("Invalid username or password.");
        }

        // UserDto, so it is safe to send back
        UserDto dto = new()
        {
            Id = user.Id,
            UserName = user.UserName
        };
        return Ok(dto);
    }
}
