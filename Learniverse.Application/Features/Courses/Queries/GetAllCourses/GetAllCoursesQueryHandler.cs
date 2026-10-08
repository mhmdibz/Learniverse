using Learniverse.Application.Common.Constants;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;

namespace Learniverse.Application.Features.Courses.Queries.GetAllCourses;

public sealed class GetAllCoursesQueryHandler
    : IRequestHandler<
        GetAllCoursesQuery,
        IReadOnlyList<GetAllCoursesResponse>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAllCoursesQueryHandler(
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    {
        _courseRepository = courseRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<GetAllCoursesResponse>> Handle(
    GetAllCoursesQuery request,
    CancellationToken cancellationToken)
    {
        var roles = _currentUserService.Roles;

        var isAdmin = roles.Contains(Roles.Admin);

        var instructorId = roles.Contains(Roles.Instructor)
            ? _currentUserService.UserIdOrNull
            : null;
        var courses = await _courseRepository.GetAllAsync(
    isAdmin,
    instructorId,
    cancellationToken);

        return courses
            .Select(course => new GetAllCoursesResponse(
                course.Id,
                course.Title,
                course.Price,
                course.Status,
                course.InstructorId,
                course.CategoryId,
                course.CategoryName,
                course.CreatedAtUtc))
            .ToList();
    }
}