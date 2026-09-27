using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Command.UpdateSection;

public sealed record UpdateSectionCommand(Guid SectionId,Guid CourseId, string Title, string Description, int Order) : IRequest;

