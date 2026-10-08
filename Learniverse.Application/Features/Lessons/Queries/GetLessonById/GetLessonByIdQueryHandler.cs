using Learniverse.Application.Common.Constants;
using Learniverse.Application.Exceptions;
using Learniverse.Application.Features.Lessons.Queries.GetLessonById;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enums;
using MediatR;

namespace Learniverse.Application.Features.Lessons.Queries.GetLessonById;

public sealed class GetLessonByIdQueryHandler
    : IRequestHandler<GetLessonByIdQuery, GetLessonByIdResponse>
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetLessonByIdQueryHandler(
        ILessonRepository lessonRepository,
        ISectionRepository sectionRepository,
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    {
        _lessonRepository = lessonRepository;
        _sectionRepository = sectionRepository;
        _courseRepository = courseRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetLessonByIdResponse> Handle(
        GetLessonByIdQuery request,
        CancellationToken cancellationToken)
    {
        var lesson =
            await _lessonRepository.GetByIdIncludingUnpublishedAsync(
                request.Id,
                cancellationToken);

        if (lesson is null)
        {
            throw new NotFoundException(nameof(Lesson), request.Id);
        }

        var section =
            await _sectionRepository.GetByIdIncludingUnpublishedAsync(
                lesson.SectionId,
                cancellationToken);

        if (section is null)
        {
            throw new NotFoundException(nameof(Section), lesson.SectionId);
        }

        var course = await _courseRepository.GetByIdAsync(
            section.CourseId,
            cancellationToken);

        if (course is null)
        {
            throw new NotFoundException(nameof(Course), section.CourseId);
        }

        var roles = _currentUserService.Roles;

        var isAdmin = roles.Contains(Roles.Admin);

        var isCourseOwner =
            roles.Contains(Roles.Instructor) &&
            _currentUserService.UserIdOrNull == course.InstructorId;

        var isPublicPreview =
            lesson.IsPreview &&
            course.Status == CourseStatus.Published;

        if (!isAdmin && !isCourseOwner && !isPublicPreview)
        {
            throw new NotFoundException(nameof(Lesson), request.Id);
        }

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