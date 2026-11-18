using System.Security.Cryptography;
using System.Text;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Identity;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Learniverse.Infrastructure.Authentication;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RefreshTokenOptions _options;

    public RefreshTokenService(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<RefreshTokenOptions> options)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _userManager = userManager;
        _signInManager = signInManager;
        _options = options.Value;
    }

    public async Task<RefreshTokenSession> IssueAsync(
        AuthenticatedUser user,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var now = DateTime.UtcNow;
        var absoluteExpiresAtUtc = now.AddDays(
            _options.MaximumSessionAgeDays);
        var expiresAtUtc = GetIdleExpiry(now, absoluteExpiresAtUtc);
        var refreshToken = GenerateToken();
        var refreshTokenHash = HashToken(refreshToken);
        var familyId = Guid.CreateVersion7();

        var tokenRecord = RefreshToken.Create(
            user.UserId,
            refreshTokenHash,
            familyId,
            now,
            expiresAtUtc,
            absoluteExpiresAtUtc);

        await _refreshTokenRepository.AddAsync(
            tokenRecord,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CreateSession(
            user,
            refreshToken,
            expiresAtUtc,
            absoluteExpiresAtUtc);
    }

    public async Task<RefreshTokenSession?> RotateAsync(
        string refreshToken,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return null;
        }

        var tokenHash = HashToken(refreshToken);
        var tokenRecord = await _refreshTokenRepository.GetByTokenHashAsync(
            tokenHash,
            cancellationToken);

        if (tokenRecord is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;

        if (tokenRecord.RevokedAtUtc is not null)
        {
            await _refreshTokenRepository.RevokeActiveFamilyAsync(
                tokenRecord.FamilyId,
                now,
                cancellationToken);

            return null;
        }

        if (!tokenRecord.IsUsableAt(now))
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(tokenRecord.UserId);

        if (user is null ||
            !await _signInManager.CanSignInAsync(user) ||
            await _userManager.IsLockedOutAsync(user))
        {
            await _refreshTokenRepository.RevokeActiveFamilyAsync(
                tokenRecord.FamilyId,
                now,
                cancellationToken);

            return null;
        }

        var roles = await _userManager.GetRolesAsync(user);
        var authenticatedUser = new AuthenticatedUser(
            user.Id,
            roles.ToArray());

        var replacementToken = GenerateToken();
        var replacementHash = HashToken(replacementToken);
        var replacementExpiry = GetIdleExpiry(
            now,
            tokenRecord.AbsoluteExpiresAtUtc);
        var replacementRecord = RefreshToken.Create(
            tokenRecord.UserId,
            replacementHash,
            tokenRecord.FamilyId,
            now,
            replacementExpiry,
            tokenRecord.AbsoluteExpiresAtUtc);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            tokenRecord.Revoke(now, replacementHash);
            await _refreshTokenRepository.AddAsync(
                replacementRecord,
                cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            await _unitOfWork.RollbackTransactionAsync(
                CancellationToken.None);
            await _refreshTokenRepository.RevokeActiveFamilyAsync(
                tokenRecord.FamilyId,
                now,
                CancellationToken.None);

            return null;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(
                CancellationToken.None);
            throw;
        }

        return CreateSession(
            authenticatedUser,
            replacementToken,
            replacementExpiry,
            tokenRecord.AbsoluteExpiresAtUtc);
    }

    public async Task RevokeAsync(
        string? refreshToken,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        var tokenRecord = await _refreshTokenRepository.GetByTokenHashAsync(
            HashToken(refreshToken),
            cancellationToken);

        if (tokenRecord is null || tokenRecord.RevokedAtUtc is not null)
        {
            return;
        }

        tokenRecord.Revoke(DateTime.UtcNow);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            await _refreshTokenRepository.RevokeActiveFamilyAsync(
                tokenRecord.FamilyId,
                DateTime.UtcNow,
                CancellationToken.None);
        }
    }

    private RefreshTokenSession CreateSession(
        AuthenticatedUser user,
        string refreshToken,
        DateTime refreshTokenExpiresAtUtc,
        DateTime sessionAbsoluteExpiresAtUtc)
    {
        var accessToken = _tokenService.GenerateToken(user);

        return new RefreshTokenSession(
            accessToken.AccessToken,
            accessToken.ExpiresAtUtc,
            refreshToken,
            new DateTimeOffset(refreshTokenExpiresAtUtc),
            new DateTimeOffset(sessionAbsoluteExpiresAtUtc));
    }

    private DateTime GetIdleExpiry(
        DateTime utcNow,
        DateTime absoluteExpiresAtUtc)
    {
        var idleExpiry = utcNow.AddDays(
            _options.InactivityExpirationDays);

        return idleExpiry < absoluteExpiresAtUtc
            ? idleExpiry
            : absoluteExpiresAtUtc;
    }

    private static string GenerateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string HashToken(string token)
    {
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
