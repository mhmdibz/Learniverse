using Learniverse.Application.Common.DTOs;
using Learniverse.Domain.Entities;

namespace Learniverse.Application.Interfaces.Repositories;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<LessonListDto>> GetAllAsync(
        CancellationToken cancellationToken);
    Task<IReadOnlyList<LessonListDto>> GetAllByCourseIdAsync(
    Guid courseId,
    CancellationToken cancellationToken);
    Task<Lesson?> GetByIdForCourseAsync(
    Guid courseId,
    Guid lessonId,
    CancellationToken cancellationToken);
}