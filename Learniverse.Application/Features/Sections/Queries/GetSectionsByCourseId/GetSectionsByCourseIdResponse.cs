using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Queries.GetSectionsByCourseId;

public sealed record GetSectionsByCourseIdResponse(
    Guid Id,
    string Title,
    string Description,
    int Order,
    Guid CourseId);