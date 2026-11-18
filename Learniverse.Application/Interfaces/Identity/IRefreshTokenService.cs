namespace Learniverse.Application.Interfaces.Identity;

public interface IRefreshTokenService
{
    Task<RefreshTokenSession> IssueAsync(
        AuthenticatedUser user,
        CancellationToken cancellationToken);

    Task<RefreshTokenSession?> RotateAsync(
        string refreshToken,
        CancellationToken cancellationToken);

    Task RevokeAsync(
        string? refreshToken,
        CancellationToken cancellationToken);
}
