using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.ResendEmailConfirmation;

public sealed record ResendEmailConfirmationCommand(string Email) : IRequest<Unit>;
