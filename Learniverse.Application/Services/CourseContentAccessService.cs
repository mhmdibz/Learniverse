using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enums;

namespace Learniverse.Application.Services;

public sealed class CourseContentAccessService
    : ICourseContentAccessService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseVisibilityPolicy _courseVisibilityPolicy;
    private readonly ICurrentUserService _currentUserService;

    public CourseContentAccessService(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        ICourseVisibilityPolicy courseVisibilityPolicy,
        ICurrentUserService currentUserService)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _courseVisibilityPolicy = courseVisibilityPolicy;
        _currentUserService = currentUserService;
    }

    public async Task EnsureCanReadAllLessonsAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(
            courseId,
            cancellationToken);

        if (course is null)
        {
            throw new NotFoundException(nameof(Course), courseId);
        }

        if (_courseVisibilityPolicy.IsAdminOrOwner(course.InstructorId))
        {
            return;
        }

        var courseAllowsStudentAccess =
            course.Status is CourseStatus.Published or CourseStatus.Archived;

        var isEnrolled = courseAllowsStudentAccess &&
            await _enrollmentRepository.HasAccessAsync(
                _currentUserService.UserId,
                courseId,
                cancellationToken);

        if (!isEnrolled)
        {
            throw new ForbiddenException(
                "You are not allowed to access this course content.");
        }
    }
}