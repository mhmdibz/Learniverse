namespace Learniverse.Application.Interfaces.Identity;

public sealed record AuthenticatedUser(
    string UserId,
    IReadOnlyList<string> Roles);