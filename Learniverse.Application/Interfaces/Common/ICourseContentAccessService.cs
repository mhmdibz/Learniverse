using Learniverse.Domain.Entities;

namespace Learniverse.Application.Interfaces.Common;

public interface ICourseContentAccessService
{
    Task<bool> CanViewCourseAsync(
        Course course,
        CancellationToken cancellationToken);

    Task EnsureCanReadAllLessonsAsync(
        Guid courseId,
        CancellationToken cancellationToken);
}