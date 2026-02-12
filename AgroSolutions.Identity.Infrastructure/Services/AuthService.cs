using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AgroSolutions.Identity.Infrastructure.Services;

public class AuthService(IConfiguration configuration) : IAuthService
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateToken(User user)
    {
        string issuer = _configuration["Jwt:Issuer"]!;
        string audience = _configuration["Jwt:Audience"]!;
        string key = _configuration["Jwt:Secret"]!;

        Log.Information("Creating the token for the user with ID {UserId}.", user.UserId);
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(key));
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Audience = audience,
            Issuer = issuer,
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature),
            Subject = new([
                new(nameof(user.UserId), user.UserId.ToString()!),
                new(nameof(user.Name), user.Name),
                new(nameof(user.Email), user.Email)
            ])
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
