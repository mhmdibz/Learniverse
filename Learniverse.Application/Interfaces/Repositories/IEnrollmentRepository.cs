namespace Learniverse.Application.Interfaces.Repositories;

public interface IEnrollmentRepository
{
    Task<bool> HasAccessAsync(
        string studentId,
        Guid courseId,
        CancellationToken cancellationToken);
}