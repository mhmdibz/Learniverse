using Learniverse.Application.Common.DTOs;
using Learniverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Interfaces.Repositories;

public interface ISectionRepository
{
    Task<Section?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<SectionListDto>> GetAllAsync(
        CancellationToken cancellationToken);
    Task<IReadOnlyList<SectionListDto>> GetByCourseIdAsync(
        Guid courseId,
        CancellationToken cancellationToken);
    Task<Section?> GetByIdWithLessonsAsync(
    Guid id,
    CancellationToken cancellationToken);
}