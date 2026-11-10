using System;
using Learniverse.Domain.Enum;
using Learniverse.Domain.Enums;

namespace Learniverse.Application.Features.Enrollments.Queries.GetMyEnrollments;

public sealed record GetMyEnrollmentsResponse(
    Guid CourseId,
    string CourseTitle,
    decimal Price,
    CourseStatus CourseStatus,
    EnrollmentStatus EnrollmentStatus,
    DateTime EnrolledAtUtc,
    DateTime? CompletedAtUtc);
