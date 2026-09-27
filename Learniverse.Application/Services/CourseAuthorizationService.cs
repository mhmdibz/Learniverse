using Learniverse.Application.Common.Constants;
using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Domain.Entities;
 namespace Learniverse.Application.Services;

public sealed class CourseAuthorizationService
    : ICourseAuthorizationService
{
    private readonly ICurrentUserService _currentUserService;

    public CourseAuthorizationService(
        ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public void EnsureCanModify(Course course)
    {
        if (_currentUserService.Roles.Contains(Roles.Admin))
        {
            return;
        }

        if (course.InstructorId == _currentUserService.UserId)
        {
            return;
        }

        throw new ForbiddenException(
            "You do not have permission to modify this course.");
    }
}