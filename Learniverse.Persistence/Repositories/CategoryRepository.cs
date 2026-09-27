using Learniverse.Application.Common.DTOs;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Learniverse.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AnyAsync(
                c => c.Id == categoryId,
                cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AnyAsync(
                c => c.Name == name,
                cancellationToken);
    }

    public async Task AddAsync(
        Category category,
        CancellationToken cancellationToken)
    {
        await _context.Categories.AddAsync(
            category,
            cancellationToken);
    }

    public async Task<CategoryDetailsDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryDetailsDto(
                c.Id,
                c.Name,
                c.CreatedBy,
                c.CreatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CategoryListDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryListDto(
                c.Id,
                c.Name,
                c.CreatedBy,
                c.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}