using FluentValidation;

namespace Learniverse.Application.Features.Identity.Commands.ResendEmailConfirmation;

public sealed class ResendEmailConfirmationCommandValidator
    : AbstractValidator<ResendEmailConfirmationCommand>
{
    public ResendEmailConfirmationCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}
