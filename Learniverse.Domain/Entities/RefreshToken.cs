using Learniverse.Domain.Common;
using Learniverse.Domain.Exceptions;

namespace Learniverse.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    private RefreshToken()
    {
    }

    private RefreshToken(
        string userId,
        string tokenHash,
        Guid familyId,
        DateTime createdAtUtc,
        DateTime expiresAtUtc,
        DateTime absoluteExpiresAtUtc)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        TokenHash = tokenHash;
        FamilyId = familyId;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        AbsoluteExpiresAtUtc = absoluteExpiresAtUtc;
    }

    public string UserId { get; private set; } = default!;
    public string TokenHash { get; private set; } = default!;
    public Guid FamilyId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime AbsoluteExpiresAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public string? ReplacedByTokenHash { get; private set; }

    public static RefreshToken Create(
        string userId,
        string tokenHash,
        Guid familyId,
        DateTime createdAtUtc,
        DateTime expiresAtUtc,
        DateTime absoluteExpiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("Refresh token user ID is required.");

        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new DomainException("Refresh token hash is required.");

        if (familyId == Guid.Empty)
            throw new DomainException("Refresh token family ID is required.");

        if (expiresAtUtc <= createdAtUtc)
            throw new DomainException(
                "Refresh token expiry must be after its creation time.");

        if (absoluteExpiresAtUtc < expiresAtUtc)
            throw new DomainException(
                "Refresh token expiry cannot exceed its session expiry.");

        return new RefreshToken(
            userId.Trim(),
            tokenHash,
            familyId,
            createdAtUtc,
            expiresAtUtc,
            absoluteExpiresAtUtc);
    }

    public bool IsUsableAt(DateTime utcNow)
    {
        return RevokedAtUtc is null &&
            utcNow < ExpiresAtUtc &&
            utcNow < AbsoluteExpiresAtUtc;
    }

    public void Revoke(DateTime revokedAtUtc, string? replacedByTokenHash = null)
    {
        if (RevokedAtUtc is not null)
            return;

        RevokedAtUtc = revokedAtUtc;
        ReplacedByTokenHash = replacedByTokenHash;
    }
}
