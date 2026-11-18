namespace Learniverse.Application.Interfaces.Identity;

public sealed record RefreshTokenSession(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc,
    DateTimeOffset SessionAbsoluteExpiresAtUtc);
