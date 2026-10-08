using Learniverse.Application.Common.Constants;
using Learniverse.Application.Exceptions;
using Learniverse.Application.Features.Sections.Queries.GetSectionById;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enums;
using MediatR;

namespace Learniverse.Application.Features.Sections.Queries.GetSectionById;

public sealed class GetSectionByIdQueryHandler
    : IRequestHandler<GetSectionByIdQuery, GetSectionByIdResponse>
{
    private readonly ISectionRepository _sectionRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetSectionByIdQueryHandler(
        ISectionRepository sectionRepository,
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    {
        _sectionRepository = sectionRepository;
        _courseRepository = courseRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetSectionByIdResponse> Handle(
        GetSectionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var section =
            await _sectionRepository.GetByIdIncludingUnpublishedAsync(
                request.Id,
                cancellationToken);

        if (section is null)
        {
            throw new NotFoundException(nameof(Section), request.Id);
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

        if (course.Status != CourseStatus.Published &&
            !isAdmin &&
            !isCourseOwner)
        {
            throw new NotFoundException(nameof(Section), request.Id);
        }

        return new GetSectionByIdResponse(
            section.Id,
            section.Title,
            section.Description,
            section.Order,
            section.CourseId);
    }
}