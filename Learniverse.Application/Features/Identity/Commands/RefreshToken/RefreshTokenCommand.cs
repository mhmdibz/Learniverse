using Learniverse.Application.Interfaces.Identity;
using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Token)
    : IRequest<RefreshTokenSession?>;
