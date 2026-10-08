using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Learniverse.Infrastructure.Authentication;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string UserId =>
        _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? throw new UnauthorizedException();
    public IReadOnlyList<string> Roles =>
    _httpContextAccessor.HttpContext?
        .User
        .FindAll(ClaimTypes.Role)
        .Select(claim => claim.Value)
        .ToArray()
    ?? Array.Empty<string>();
    public bool IsAuthenticated =>
    _httpContextAccessor.HttpContext?
        .User.Identity?.IsAuthenticated == true;

    public string? UserIdOrNull =>
        IsAuthenticated
            ? _httpContextAccessor.HttpContext?
                .User.FindFirstValue(JwtRegisteredClaimNames.Sub)
            : null;

}