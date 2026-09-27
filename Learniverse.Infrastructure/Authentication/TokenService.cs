using System.Security.Claims;
using System.Text;
using Learniverse.Application.Interfaces.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Learniverse.Infrastructure.Authentication;

public sealed class TokenService : ITokenService
{
    private readonly JwtOptions _options;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public TokenResult GenerateToken(AuthenticatedUser user)
    {
        var expiresAtUtc =
            DateTimeOffset.UtcNow.AddMinutes(
                _options.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId)
        };

        foreach (var role in user.Roles)
        {
            claims.Add(
                new(ClaimTypes.Role, role));
        }

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SecretKey));

        var signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Expires = expiresAtUtc.UtcDateTime,
            SigningCredentials = signingCredentials
        };

        var handler = new JsonWebTokenHandler();

        var accessToken = handler.CreateToken(descriptor);

        return new TokenResult(
            accessToken,
            expiresAtUtc);
    }
}