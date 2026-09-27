using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.Login
{
    public sealed record LoginCommand(string Identifier, string Password) : IRequest<LoginResult>;
}
