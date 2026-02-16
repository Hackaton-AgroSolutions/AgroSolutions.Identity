using AgroSolutions.Identity.Domain.Entities;
using System.Security.Claims;

namespace AgroSolutions.Identity.API.Extensions;

public static class ClaimsExtensions
{
    extension(ClaimsPrincipal principal)
    {
        public int UserId => int.Parse(principal.Claims.First(c => c.Type == nameof(User.UserId)).Value);
    }
}
