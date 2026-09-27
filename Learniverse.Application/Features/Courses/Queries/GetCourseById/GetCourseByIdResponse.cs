using Learniverse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Courses.Queries.GetCourseById
{
    public sealed record GetCourseByIdResponse(
     Guid Id,
     string Title,
     string Description,
     decimal Price,
     CourseStatus Status,
     string InstructorId,
     Guid CategoryId,
     string CategoryName,
     DateTime CreatedAtUtc,
     DateTime? UpdatedAtUtc
 );
}
