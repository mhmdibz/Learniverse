using FluentValidation;

namespace Learniverse.Application.Features.Lessons.Commands.UpdateLesson;

public sealed class UpdateLessonCommandValidator
    : AbstractValidator<UpdateLessonCommand>
{
    public UpdateLessonCommandValidator()
    {
        RuleFor(x => x.SectionId)
            .NotEmpty()
            .WithMessage("SectionId is required.");

        RuleFor(x => x.LessonId)
            .NotEmpty()
            .WithMessage("LessonId is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Lesson title is required.")
            .MaximumLength(200)
            .WithMessage("Lesson title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Lesson description is required.")
            .MaximumLength(2000)
            .WithMessage("Lesson description must not exceed 2000 characters.");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Lesson order must be greater than zero.");

        RuleFor(x => x.ContentType)
            .IsInEnum()
            .WithMessage("Invalid content type.");
    }
}