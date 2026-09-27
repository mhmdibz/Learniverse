using FluentValidation;

namespace Learniverse.Application.Features.Courses.Queries.GetCourseById;

public sealed class GetCourseByIdQueryValidator
    : AbstractValidator<GetCourseByIdQuery>
{
    public GetCourseByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Course id is required.");
    }
}