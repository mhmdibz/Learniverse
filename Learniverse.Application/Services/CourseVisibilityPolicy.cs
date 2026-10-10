using Learniverse.Application.Common.Constants;
using Learniverse.Application.Interfaces.Common;

namespace Learniverse.Application.Services;

public sealed class CourseVisibilityPolicy : ICourseVisibilityPolicy
{
    private readonly ICurrentUserService _currentUserService;

    public CourseVisibilityPolicy(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public bool IsAdminOrOwner(string courseInstructorId)
    {
        var roles = _currentUserService.Roles;

        return roles.Contains(Roles.Admin) ||
            (roles.Contains(Roles.Instructor) &&
             _currentUserService.UserIdOrNull == courseInstructorId);
    }
}