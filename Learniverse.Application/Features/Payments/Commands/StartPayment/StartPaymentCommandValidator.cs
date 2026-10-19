using FluentValidation;

namespace Learniverse.Application.Features.Payments.Commands.StartPayment;

public sealed class StartPaymentCommandValidator
    : AbstractValidator<StartPaymentCommand>
{
    public StartPaymentCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Course ID is required.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .MaximumLength(32)
            .WithMessage("Phone number must not exceed 32 characters.");
    }
}