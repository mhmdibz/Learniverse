using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Command.CreateSection;

public sealed class CreateSectionCommandValidator
 : AbstractValidator<CreateSectionCommand>
{
    public CreateSectionCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("Course id is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Section title is required.")
            .MaximumLength(200)
            .WithMessage("Section title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Section description is required.");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Section order must be greater than zero.");
    }
}