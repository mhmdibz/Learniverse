using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.Register;

public sealed record RegisterCommand(
    string FullName,
    string UserName,
    string Email,
    string Password
) : IRequest<string>;