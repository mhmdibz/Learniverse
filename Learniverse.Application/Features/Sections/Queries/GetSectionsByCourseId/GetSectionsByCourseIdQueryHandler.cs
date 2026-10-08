using Learniverse.Application.Common.Constants;
using Learniverse.Application.Exceptions;
using Learniverse.Application.Features.Sections.Queries.GetSectionsByCourseId;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enums;
using MediatR;

namespace Learniverse.Application.Features.Sections.Queries.GetSectionsByCourseId;

public sealed class GetSectionsByCourseIdQueryHandler
    : IRequestHandler<
        GetSectionsByCourseIdQuery,
        IReadOnlyList<GetSectionsByCourseIdResponse>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetSectionsByCourseIdQueryHandler(
        ICourseRepository courseRepository,
        ISectionRepository sectionRepository,
        ICurrentUserService currentUserService)
    {
        _courseRepository = courseRepository;
        _sectionRepository = sectionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<GetSectionsByCourseIdResponse>> Handle(
        GetSectionsByCourseIdQuery request,
        CancellationToken cancellationToken)
    {
         var course = await _courseRepository.GetByIdAsync(
            request.CourseId,
            cancellationToken);

        if (course is null)
        {
            throw new NotFoundException(
                nameof(Course),
                request.CourseId);
        }

        var roles = _currentUserService.Roles;

        var isAdmin = roles.Contains(Roles.Admin);

        var isCourseOwner =
            roles.Contains(Roles.Instructor) &&
            _currentUserService.UserIdOrNull == course.InstructorId;

         if (course.Status != CourseStatus.Published &&
            !isAdmin &&
            !isCourseOwner)
        {
            throw new NotFoundException(
                nameof(Course),
                request.CourseId);
        }

        
        var sections =
            await _sectionRepository.GetByCourseIdIncludingUnpublishedAsync(
                request.CourseId,
                cancellationToken);

        return sections
            .Select(s => new GetSectionsByCourseIdResponse(
                s.Id,
                s.Title,
                s.Description,
                s.Order,
                s.CourseId))
            .ToList();
    }
}