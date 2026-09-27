using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Command.CreateSection
{
    public sealed record CreateSectionCommand(
    Guid CourseId,
    string Title,
    string Description,
    int Order
) : IRequest<Guid>;
}
