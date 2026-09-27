using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;

namespace Learniverse.Application.Features.Lessons.Queries.GetLessonById;

public sealed class GetLessonByIdQueryHandler
    : IRequestHandler<GetLessonByIdQuery, GetLessonByIdResponse>
{
    private readonly ILessonRepository _lessonRepository;

    public GetLessonByIdQueryHandler(
        ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }

    public async Task<GetLessonByIdResponse> Handle(
        GetLessonByIdQuery request,
        CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (lesson is null)
            throw new NotFoundException(
                nameof(Domain.Entities.Lesson),
                request.Id);

        return new GetLessonByIdResponse(
            lesson.Id,
            lesson.Title,
            lesson.Description,
            lesson.Order,
            lesson.ContentType,
            lesson.SectionId,
            lesson.IsPreview);
    }
}