namespace Learniverse.Application.Interfaces.Identity;

public sealed record TokenResult(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc);