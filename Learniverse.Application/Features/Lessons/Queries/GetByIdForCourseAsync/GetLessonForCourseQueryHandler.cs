using Learniverse.Application.Exceptions;
using Learniverse.Application.Features.Lessons.Queries.GetLessonById;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using MediatR;

namespace Learniverse.Application.Features.Lessons.Queries.GetLessonForCourse;

public sealed class GetLessonForCourseQueryHandler
    : IRequestHandler<GetLessonForCourseQuery, GetLessonForCourseResponse>
{
    private readonly ICourseContentAccessService _accessService;
    private readonly ILessonRepository _lessonRepository;

    public GetLessonForCourseQueryHandler(
        ICourseContentAccessService accessService,
        ILessonRepository lessonRepository)
    {
        _accessService = accessService;
        _lessonRepository = lessonRepository;
    }

    public async Task<GetLessonForCourseResponse> Handle(
        GetLessonForCourseQuery request,
        CancellationToken cancellationToken)
    {
        await _accessService.EnsureCanReadAllLessonsAsync(
            request.CourseId,
            cancellationToken);

        var lesson = await _lessonRepository.GetByIdForCourseAsync(
            request.CourseId,
            request.LessonId,
            cancellationToken);

        if (lesson is null)
            throw new NotFoundException(nameof(Lesson), request.LessonId);

        return new GetLessonForCourseResponse(
            lesson.Id,
            lesson.Title,
            lesson.Description,
            lesson.Order,
            lesson.ContentType,
            lesson.SectionId,
            lesson.IsPreview);
    }
}