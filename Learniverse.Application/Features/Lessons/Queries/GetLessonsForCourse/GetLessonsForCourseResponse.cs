using Learniverse.Domain.Enums;

namespace Learniverse.Application.Features.Lessons.Queries.GetLessonsForCourse;

public sealed record GetLessonsForCourseResponse(
    Guid Id,
    string Title,
    string Description,
    int Order,
    ContentType ContentType,
    Guid SectionId,
    bool IsPreview);