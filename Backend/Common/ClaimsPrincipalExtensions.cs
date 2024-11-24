using Backend.Contract.Authenticate.SessionCheck;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Backend.Common;

static class ClaimsPrincipalExtensions
{
    public static GetResponse ToSessionCheckResponse(this ClaimsPrincipal user)
    {
        return new GetResponse(
            user.FindFirstValue(ClaimTypes.Email) ?? "",
            user.FindFirstValue(ClaimTypes.Name) ?? "");
    }
}
