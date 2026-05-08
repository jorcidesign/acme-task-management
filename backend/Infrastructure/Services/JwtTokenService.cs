using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AcmeTaskApi.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
 
namespace AcmeTaskApi.Infrastructure.Services;
 
public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";
    public string Secret { get; init; } = string.Empty;
    public int ExpiresInHours { get; init; } = 8;
    public string Issuer { get; init; } = "AcmeTaskApi";
    public string Audience { get; init; } = "AcmeTaskClient";
}
 
public sealed class JwtTokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly SymmetricSecurityKey _signingKey;
 
    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
    }
 
    public string GenerateToken(long userId, string email, string fullName)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Name, fullName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                      DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                      ClaimValueTypes.Integer64)
        };
 
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(_settings.ExpiresInHours),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)
        };
 
        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(handler.CreateToken(descriptor));
    }
}
 