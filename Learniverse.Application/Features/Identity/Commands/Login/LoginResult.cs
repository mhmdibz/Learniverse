namespace Learniverse.Application.Features.Identity.Commands.Login;

public sealed record LoginResult(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc,
    DateTimeOffset SessionAbsoluteExpiresAtUtc);
