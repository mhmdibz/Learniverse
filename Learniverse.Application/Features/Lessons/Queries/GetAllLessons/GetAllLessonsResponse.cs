using Learniverse.Domain.Enums;

namespace Learniverse.Application.Features.Lessons.Queries.GetAllLessons;

public sealed record GetAllLessonsResponse(
    Guid Id,
    string Title,
    string Description,
    int Order,
    ContentType ContentType,
    Guid SectionId,
    bool IsPreview
);