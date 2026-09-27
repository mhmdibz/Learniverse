using Learniverse.Domain.Enums;

namespace Learniverse.Application.Common.DTOs;

public sealed record LessonListDto(
    Guid Id,
    string Title,
    string Description,
    int Order,
    ContentType ContentType,
    Guid SectionId,
    bool IsPreview
);