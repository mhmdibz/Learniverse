using FluentValidation;

namespace Learniverse.Application.Features.Sections.Queries.GetSectionById;

public sealed class GetSectionByIdQueryValidator
    : AbstractValidator<GetSectionByIdQuery>
{
    public GetSectionByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Section id is required.");
    }
}