using FluentValidation.Results;
using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Identity;
using MediatR;

namespace Learniverse.Application.Features.Identity.Commands.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, string>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<string> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _identityService.RegisterStudentAsync(
            request.FullName,
            request.UserName,
            request.Email,
            request.Password,
            cancellationToken);

        if (!result.Succeeded)
        {
            throw new ValidationException(
                result.Errors.Select(error =>
                    new ValidationFailure("Identity", error)));
        }

        return result.UserId!;
    }
}