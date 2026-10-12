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

    public async Task<bool> CanViewCourseAsync(
        Course course,
        CancellationToken cancellationToken)
    {
        if (_courseVisibilityPolicy.IsAdminOrOwner(course.InstructorId))
        {
            return true;
        }

        if (course.Status == CourseStatus.Published)
        {
            return true;
        }

        if (course.Status != CourseStatus.Archived)
        {
            return false;
        }

        return await HasEnrollmentAccessAsync(
            course.Id,
            cancellationToken);
    }

    public async Task EnsureCanReadAllLessonsAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(
            courseId,
            cancellationToken);

        if (course is null ||
            !await CanViewCourseAsync(course, cancellationToken))
        {
            throw new NotFoundException(nameof(Course), courseId);
        }

        if (_courseVisibilityPolicy.IsAdminOrOwner(course.InstructorId))
        {
            return;
        }

        if (await HasEnrollmentAccessAsync(courseId, cancellationToken))
        {
            return;
        }

        throw new ForbiddenException(
            "You are not allowed to access this course content.");
    }

    private async Task<bool> HasEnrollmentAccessAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserIdOrNull;

        if (userId is null)
        {
            return false;
        }

        return await _enrollmentRepository.HasAccessAsync(
            userId,
            courseId,
            cancellationToken);
    }
}