using Learniverse.Application.Common.DTOs;
using Learniverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Interfaces.Repositories
{
    public interface ICourseRepository
    {
        Task<bool> ExistsAsync(Guid courseId, CancellationToken cancellationToken);
        Task AddAsync(Course course, CancellationToken cancellationToken);
        Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
         Task<IReadOnlyList<CourseListDto>> GetAllAsync(
            CancellationToken cancellationToken);
        Task<Course?> GetByIdWithSectionsAsync(
    Guid id,
    CancellationToken cancellationToken);
    }
}
