using Learniverse.Domain.Enums;

namespace Learniverse.Application.Common.DTOs;

public sealed record CourseListDto(
    Guid Id,
    string Title,
    decimal Price,
    CourseStatus Status,
    string InstructorId,
    Guid CategoryId,
    string CategoryName,
    DateTime CreatedAtUtc
);