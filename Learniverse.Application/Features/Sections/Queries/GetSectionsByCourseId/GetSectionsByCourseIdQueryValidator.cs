using System;
using System.Collections.Generic;
using System.Text;

using FluentValidation;

namespace Learniverse.Application.Features.Sections.Queries.GetSectionsByCourseId;

public sealed class GetSectionsByCourseIdQueryValidator
    : AbstractValidator<GetSectionsByCourseIdQuery>
{
    public GetSectionsByCourseIdQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Course id is required.");
    }
}