using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.Logout;

public sealed record LogoutCommand(string? RefreshToken) : IRequest<Unit>;
