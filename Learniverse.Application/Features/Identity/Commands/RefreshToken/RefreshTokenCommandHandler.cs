using Learniverse.Application.Interfaces.Identity;
using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, RefreshTokenSession?>
{
    private readonly IRefreshTokenService _refreshTokenService;

    public RefreshTokenCommandHandler(
        IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
    }

    public Task<RefreshTokenSession?> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        return _refreshTokenService.RotateAsync(
            request.Token,
            cancellationToken);
    }
}
