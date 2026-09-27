using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Command.UpdateSection;

using FluentValidation;


public sealed class UpdateSectionCommandValidator
    : AbstractValidator<UpdateSectionCommand>
{
    public UpdateSectionCommandValidator()
    {
        RuleFor(x => x.SectionId)
            .NotEmpty()
            .WithMessage("Section id is required.");
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("CourseId is required.");


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