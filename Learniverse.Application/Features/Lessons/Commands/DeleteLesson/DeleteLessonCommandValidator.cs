using FluentValidation;

namespace Learniverse.Application.Features.Lessons.Commands.DeleteLesson;

public sealed class DeleteLessonCommandValidator
    : AbstractValidator<DeleteLessonCommand>
{
    public DeleteLessonCommandValidator()
    {
        RuleFor(x => x.SectionId)
            .NotEmpty()
            .WithMessage("SectionId is required.");

        RuleFor(x => x.LessonId)
            .NotEmpty()
            .WithMessage("LessonId is required.");
    }
}