using FluentValidation;
using Learniverse.Domain.Enums;

namespace Learniverse.Application.Features.Courses.Commands.UpdateCourse;

public sealed class UpdateCourseCommandValidator
    : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Course id is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Course title is required.")
            .MaximumLength(200)
            .WithMessage("Course title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Course description is required.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Course price cannot be negative.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid course status.");
    }
}