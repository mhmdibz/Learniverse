using Learniverse.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Courses.Queries.GetAllCourses;

    public sealed record GetAllCoursesResponse(
     Guid Id,
     string Title,
     decimal Price,
     CourseStatus Status,
     string InstructorId,
     Guid CategoryId,
     string CategoryName,
     DateTime CreatedAtUtc
 );

