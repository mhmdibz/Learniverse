using Learniverse.Domain.Enums;
using MediatR;

namespace Learniverse.Application.Features.Lessons.Commands.UpdateLesson;

public sealed record UpdateLessonCommand(
    Guid SectionId,
    Guid LessonId,
    string Title,
    string Description,
    int Order,
    ContentType ContentType,
    bool IsPreview
) : IRequest;