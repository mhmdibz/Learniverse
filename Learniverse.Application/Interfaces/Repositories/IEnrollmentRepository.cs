using Learniverse.Domain.Entities;

namespace Learniverse.Application.Interfaces.Repositories;

public interface IEnrollmentRepository
{
    Task<bool> HasAccessAsync(
        string studentId,
        Guid courseId,
        CancellationToken cancellationToken);
    Task<Enrollment?> GetByStudentAndCourseAsync(
    string studentId,
    Guid courseId,
    CancellationToken cancellationToken);

    Task AddAsync(
        Enrollment enrollment,
        CancellationToken cancellationToken);
}
