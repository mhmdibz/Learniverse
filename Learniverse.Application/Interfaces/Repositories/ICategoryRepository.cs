using Learniverse.Application.Common.DTOs;
using Learniverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<bool> ExistsAsync(Guid categoryId, CancellationToken cancellationToken);
    Task AddAsync(Category category, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(
    string name,
    CancellationToken cancellationToken);

    Task<CategoryDetailsDto?> GetByIdAsync(
     Guid id,
     CancellationToken cancellationToken);
    Task<IReadOnlyList<CategoryListDto>> GetAllAsync(
        CancellationToken cancellationToken);
}