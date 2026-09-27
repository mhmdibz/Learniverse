using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Sections.Queries.GetSectionById;
    public sealed record GetSectionByIdResponse(Guid Id,
        string Title,
        string Description,
        int Order,
        Guid CourseId);

