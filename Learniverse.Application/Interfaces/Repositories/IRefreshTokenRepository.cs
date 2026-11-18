using Learniverse.Domain.Entities;

namespace Learniverse.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken);

    Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<RefreshToken>> GetByFamilyIdAsync(
        Guid familyId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(
        string userId,
        DateTime utcNow,
        CancellationToken cancellationToken);

    Task<int> RevokeActiveFamilyAsync(
        Guid familyId,
        DateTime utcNow,
        CancellationToken cancellationToken);
}
