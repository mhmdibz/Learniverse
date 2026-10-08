using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;

namespace Learniverse.Application.Features.Lessons.Queries.GetLessonsForCourse;

public sealed class GetLessonsForCourseQueryHandler
    : IRequestHandler<
        GetLessonsForCourseQuery,
        IReadOnlyList<GetLessonsForCourseResponse>>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICourseContentAccessService _accessService;

    public GetLessonsForCourseQueryHandler(
        ILessonRepository lessonRepository,
        ICourseContentAccessService accessService)
    {
        _lessonRepository = lessonRepository;
        _accessService = accessService;
    }

    public async Task<IReadOnlyList<GetLessonsForCourseResponse>> Handle(
        GetLessonsForCourseQuery request,
        CancellationToken cancellationToken)
    {
        await _accessService.EnsureCanReadAllLessonsAsync(
            request.CourseId,
            cancellationToken);

        var lessons = await _lessonRepository.GetAllByCourseIdAsync(
            request.CourseId,
            cancellationToken);

        return lessons
            .Select(l => new GetLessonsForCourseResponse(
                l.Id,
                l.Title,
                l.Description,
                l.Order,
                l.ContentType,
                l.SectionId,
                l.IsPreview))
            .ToList();
    }
}