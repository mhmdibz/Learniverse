using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Command.DeleteSection;

    public sealed class DeleteSectionCommandValidator
    : AbstractValidator<DeleteSectionCommand>
    {
        public DeleteSectionCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty()
                .WithMessage("Course id is required.");

            RuleFor(x => x.SectionId)
                .NotEmpty()
                .WithMessage("Section id is required.");
        }
    }

