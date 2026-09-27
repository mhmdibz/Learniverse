using MediatR;

namespace Learniverse.Application.Features.Lessons.Commands.DeleteLesson;

public sealed record DeleteLessonCommand(
    Guid SectionId,
    Guid LessonId
) : IRequest;