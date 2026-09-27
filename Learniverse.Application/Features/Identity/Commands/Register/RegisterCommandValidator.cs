using FluentValidation;

namespace Learniverse.Application.Features.Identity.Commands.Register;

public sealed class RegisterCommandValidator
    : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.UserName)
    .NotEmpty()
    .MinimumLength(3)
    .MaximumLength(50)
    .Must(x => !x.Any(char.IsWhiteSpace))
    .WithMessage("Username cannot contain spaces.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}