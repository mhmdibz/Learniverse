using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Command.DeleteSection
{
    public sealed record DeleteSectionCommand(Guid SectionId, Guid CourseId) : IRequest;
}
