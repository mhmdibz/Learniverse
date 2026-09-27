using Learniverse.Domain.Entities;

namespace Learniverse.Application.Interfaces.Common;

public interface ICourseAuthorizationService
{
    void EnsureCanModify(Course course);
}