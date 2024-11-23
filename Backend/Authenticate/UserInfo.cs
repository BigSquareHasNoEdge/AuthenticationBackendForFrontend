using System.Security.Claims;

namespace Backend.Authenticate;
record UserInfo(string Email, string Name)
{
    public bool IsValid() =>
        !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Name);
    public static UserInfo Empty() => new(string.Empty, string.Empty);

    public ClaimsPrincipal ToPrincipal(string? authenticationType)
    {
        var claims = new[] {
            new Claim(ClaimTypes.Email, Email),
            new Claim(ClaimTypes.Name, Name)
        };

        var identity = new ClaimsIdentity(claims, authenticationType);
        return new ClaimsPrincipal(identity);
    }
};