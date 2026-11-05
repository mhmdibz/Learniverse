using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Unit>;
