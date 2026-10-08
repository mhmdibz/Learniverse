namespace Learniverse.Application.Interfaces.Common;

public interface ICourseContentAccessService
{
    Task EnsureCanReadAllLessonsAsync(
        Guid courseId,
        CancellationToken cancellationToken);
}