using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Learniverse.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken)
    {
        await _context.RefreshTokens.AddAsync(
            refreshToken,
            cancellationToken);
    }

    public Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken)
    {
        return _context.RefreshTokens.FirstOrDefaultAsync(
            refreshToken => refreshToken.TokenHash == tokenHash,
            cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetByFamilyIdAsync(
        Guid familyId,
        CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .Where(refreshToken => refreshToken.FamilyId == familyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(
        string userId,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        return await _context.RefreshTokens
            .Where(refreshToken =>
                refreshToken.UserId == userId &&
                refreshToken.RevokedAtUtc == null &&
                refreshToken.ExpiresAtUtc > utcNow &&
                refreshToken.AbsoluteExpiresAtUtc > utcNow)
            .ToListAsync(cancellationToken);
    }

    public Task<int> RevokeActiveFamilyAsync(
        Guid familyId,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        return _context.RefreshTokens
            .Where(refreshToken =>
                refreshToken.FamilyId == familyId &&
                refreshToken.RevokedAtUtc == null &&
                refreshToken.ExpiresAtUtc > utcNow &&
                refreshToken.AbsoluteExpiresAtUtc > utcNow)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        refreshToken => refreshToken.RevokedAtUtc,
                        utcNow)
                    .SetProperty(
                        refreshToken => refreshToken.ReplacedByTokenHash,
                        (string?)null),
                cancellationToken);
    }
}
