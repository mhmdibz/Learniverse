using Learniverse.Application.Common.Constants;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;

namespace Learniverse.Application.Features.Lessons.Queries.GetAllLessons;

public sealed class GetAllLessonsQueryHandler
    : IRequestHandler<
        GetAllLessonsQuery,
        IReadOnlyList<GetAllLessonsResponse>>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAllLessonsQueryHandler(
        ILessonRepository lessonRepository,
        ICurrentUserService currentUserService)
    {
        _lessonRepository = lessonRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<GetAllLessonsResponse>> Handle(
        GetAllLessonsQuery request,
        CancellationToken cancellationToken)
    {
        var roles = _currentUserService.Roles;

        var isAdmin = roles.Contains(Roles.Admin);

        var instructorId = roles.Contains(Roles.Instructor)
            ? _currentUserService.UserIdOrNull
            : null;

        var lessons = await _lessonRepository.GetAllAsync(
            isAdmin,
            instructorId,
            cancellationToken);

        return lessons
            .Select(l => new GetAllLessonsResponse(
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