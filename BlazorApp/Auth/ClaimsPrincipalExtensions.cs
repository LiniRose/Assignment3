using System.Security.Claims;

namespace BlazorApp.Auth;

public static class ClaimsPrincipalExtensions
{
    // Reads the "Id" claim
    public static int GetUserId(this ClaimsPrincipal user)
    {
        string id = user.Claims.Single(c => c.Type == "Id").Value;
        return int.Parse(id);
    }
}
