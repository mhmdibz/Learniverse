using Learniverse.Domain.Enums;

namespace Learniverse.Application.Features.Lessons.Queries.GetLessonById;

public sealed record GetLessonByIdResponse(
    Guid Id,
    string Title,
    string Description,
    int Order,
    ContentType ContentType,
    Guid SectionId,
    bool IsPreview
);