using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Identity;
using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IIdentityService _identityService;
    private readonly IRefreshTokenService _refreshTokenService;

    public LoginCommandHandler(
        IIdentityService identityService,
        IRefreshTokenService refreshTokenService)
    {
        _identityService = identityService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<LoginResult> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var authenticationResult =
            await _identityService.AuthenticateAsync(
                request.Identifier,
                request.Password,
                cancellationToken);

        if (!authenticationResult.Succeeded)
        {
            throw authenticationResult.FailureReason switch
            {
                AuthenticationFailureReason.LockedOut =>
                    new AccountLockedException(),

                AuthenticationFailureReason.NotAllowed =>
                    new AccountNotAllowedException(),

                _ =>
                    new AuthenticationFailedException()
            };
        }

        var authenticatedUser = authenticationResult.User!;

        var session = await _refreshTokenService.IssueAsync(
            authenticatedUser,
            cancellationToken);

        return new LoginResult(
            session.AccessToken,
            session.AccessTokenExpiresAtUtc,
            session.RefreshToken,
            session.RefreshTokenExpiresAtUtc,
            session.SessionAbsoluteExpiresAtUtc);
    }
}
