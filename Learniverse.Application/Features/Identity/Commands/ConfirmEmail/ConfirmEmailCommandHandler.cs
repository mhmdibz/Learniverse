using FluentValidation.Results;
using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Identity;
using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler
    : IRequestHandler<ConfirmEmailCommand, Unit>
{
    private readonly IIdentityService _identityService;

    public ConfirmEmailCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Unit> Handle(
        ConfirmEmailCommand request,
        CancellationToken cancellationToken)
    {
        var confirmed = await _identityService.ConfirmEmailAsync(
            request.Email,
            request.Token,
            cancellationToken);

        if (!confirmed)
        {
            throw new ValidationException(
                new[]
                {
                    new ValidationFailure(
                        "Token",
                        "The email confirmation token is invalid or expired.")
                });
        }

        return Unit.Value;
    }
}
