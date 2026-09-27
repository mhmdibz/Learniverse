using Learniverse.Domain.Enums;

namespace Learniverse.API.Contracts.Lessons;

public sealed record UpdateLessonRequest(
    string Title,
    string Description,
    int Order,
    ContentType ContentType,
    bool IsPreview
);