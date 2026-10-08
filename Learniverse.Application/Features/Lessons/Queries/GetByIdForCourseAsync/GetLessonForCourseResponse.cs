using Learniverse.Domain.Enums;

namespace Learniverse.Application.Features.Lessons.Queries.GetLessonForCourse;

public sealed record GetLessonForCourseResponse(
    Guid Id,
    string Title,
    string Description,
    int Order,
    ContentType ContentType,
    Guid SectionId,
    bool IsPreview);