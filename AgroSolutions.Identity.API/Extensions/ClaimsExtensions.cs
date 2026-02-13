using System.Security.Claims;

namespace AgroSolutions.Identity.API.Extensions;

public static class ClaimsExtensions
{
    public static int UserId(this ClaimsPrincipal principal)
    {
        return int.Parse(principal.Claims.First(c => c.Type == "UserId").Value);
    }
}
