namespace Learniverse.API.Contracts.Responses.Identity;

public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset ExpiresAtUtc);