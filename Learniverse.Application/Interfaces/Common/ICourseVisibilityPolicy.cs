namespace Learniverse.Application.Interfaces.Common;

public interface ICourseVisibilityPolicy
{
    bool IsAdminOrOwner(string courseInstructorId);
}