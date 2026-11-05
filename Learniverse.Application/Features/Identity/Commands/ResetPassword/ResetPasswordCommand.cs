using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.ResetPassword;

public sealed record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword) : IRequest<Unit>;
