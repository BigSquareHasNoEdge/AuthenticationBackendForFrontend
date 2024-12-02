using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Backend.Contract.Authenticate.SessionCheck;
public record SessionInfo(string Email, string Name, string Provider)
{
    public static SessionInfo Empty => new("", "", "");

    public static SessionInfo From(ClaimsPrincipal user) => 
        new ( user.FindFirst(ClaimTypes.Email)?.Value ?? "",
            user.FindFirst(ClaimTypes.Name)?.Value ?? "",
            user.FindFirst("Provider")?.Value ?? "");
}