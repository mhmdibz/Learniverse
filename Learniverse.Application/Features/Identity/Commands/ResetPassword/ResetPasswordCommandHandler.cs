using FluentValidation.Results;
using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Identity;
using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand, Unit>
{
    private readonly IIdentityService _identityService;

    public ResetPasswordCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Unit> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _identityService.ResetPasswordAsync(
            request.Email,
            request.Token,
            request.NewPassword,
            cancellationToken);

        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors.Select(error =>
                new ValidationFailure("ResetPassword", error)));
        }

        return Unit.Value;
    }
}
