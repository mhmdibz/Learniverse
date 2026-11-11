using System;
using Learniverse.Domain.Enum;
using Learniverse.Domain.Enums;

namespace Learniverse.Application.Common.DTOs;

public sealed record StudentEnrollmentDto(
    Guid CourseId,
    string CourseTitle,
    decimal Price,
    CourseStatus CourseStatus,
    EnrollmentStatus EnrollmentStatus,
    DateTime EnrolledAtUtc,
    DateTime? CompletedAtUtc);
