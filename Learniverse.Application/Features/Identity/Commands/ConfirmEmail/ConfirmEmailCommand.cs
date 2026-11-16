using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(
    string Email,
    string Token) : IRequest<Unit>;
