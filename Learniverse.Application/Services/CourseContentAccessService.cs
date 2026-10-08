using Learniverse.Application.Common.Constants;
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
    private readonly ICurrentUserService _currentUserService;

    public CourseContentAccessService(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        ICurrentUserService currentUserService)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
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
            throw new NotFoundException(nameof(Course), courseId);

        var userId = _currentUserService.UserId;
        var roles = _currentUserService.Roles;

        var isAdmin = roles.Contains(Roles.Admin);
        var isCourseOwner = course.InstructorId == userId;

         if (isAdmin || isCourseOwner)
            return;

        var courseAllowsStudentAccess =
            course.Status is CourseStatus.Published or CourseStatus.Archived;

        var isEnrolled = courseAllowsStudentAccess &&
            await _enrollmentRepository.HasAccessAsync(
                userId,
                courseId,
                cancellationToken);

        if (!isEnrolled)
            throw new ForbiddenException(
                "You are not allowed to access this course content.");
    }
}