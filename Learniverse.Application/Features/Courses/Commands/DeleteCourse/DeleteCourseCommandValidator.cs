using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Learniverse.Application.Features.Courses.Commands.DeleteCourse;

public sealed class DeleteCourseCommandValidator
    : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Course id is required.");
    }
}