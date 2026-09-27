using FluentValidation;

namespace Learniverse.Application.Features.Lessons.Queries.GetLessonById;

public sealed class GetLessonByIdQueryValidator
    : AbstractValidator<GetLessonByIdQuery>
{
    public GetLessonByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("LessonId is required.");
    }
}