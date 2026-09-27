using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Identity;
using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IIdentityService identityService,
        ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
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

        var tokenResult =
            _tokenService.GenerateToken(authenticatedUser);

        return new LoginResult(
            tokenResult.AccessToken,
            tokenResult.ExpiresAtUtc);
    }
}