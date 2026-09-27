using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
namespace Learniverse.Application.Features.Sections.Queries.GetSectionsByCourseId;

public sealed record GetSectionsByCourseIdQuery(
    Guid CourseId
) : IRequest<IReadOnlyList<GetSectionsByCourseIdResponse>>;