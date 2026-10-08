using Learniverse.Application.Common.Constants;
using Learniverse.Application.Exceptions;
using Learniverse.Application.Features.Courses.Queries.GetCourseById;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enums;
using MediatR;

namespace Learniverse.Application.Features.Courses.Queries.GetCourseById;

public sealed class GetCourseByIdQueryHandler
    : IRequestHandler<GetCourseByIdQuery, GetCourseByIdResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCourseByIdQueryHandler(
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    {
        _courseRepository = courseRepository;
        _currentUserService = currentUserService;
    }

    public async Task<GetCourseByIdResponse> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(
           request.Id,
           cancellationToken);

        if (course is null)
        {
            throw new NotFoundException(nameof(Course), request.Id);
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
            throw new NotFoundException(nameof(Course), request.Id);
        }

        return new GetCourseByIdResponse(
            course.Id,
            course.Title,
            course.Description,
            course.Price,
            course.Status,
            course.InstructorId,
            course.CategoryId,
            course.Category.Name,
            course.CreatedAtUtc,
            course.UpdatedAtUtc);
    }
}