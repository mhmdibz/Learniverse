namespace Learniverse.API.Contracts.Requests.Identity;

public sealed record LoginRequest(
    string Identifier,
    string Password);