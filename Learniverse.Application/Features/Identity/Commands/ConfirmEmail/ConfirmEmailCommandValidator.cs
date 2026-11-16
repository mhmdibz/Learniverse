using FluentValidation;

namespace Learniverse.Application.Features.Identity.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandValidator
    : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(command => command.Token)
            .NotEmpty();
    }
}
