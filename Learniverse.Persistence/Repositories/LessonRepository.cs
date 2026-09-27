using Learniverse.Application.Common.DTOs;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Learniverse.Persistence.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _context;

    public LessonRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Lesson?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Lessons
            .FirstOrDefaultAsync(
                l => l.Id == id,
                cancellationToken);
    }
    public async Task<IReadOnlyList<LessonListDto>> GetAllAsync(
    CancellationToken cancellationToken)
    {
        return await _context.Lessons
            .AsNoTracking()
            .OrderBy(l => l.SectionId)
            .ThenBy(l => l.Order)
            .Select(l => new LessonListDto(
                l.Id,
                l.Title,
                l.Description,
                l.Order,
                l.ContentType,
                l.SectionId,
                l.IsPreview))
            .ToListAsync(cancellationToken);
    }
}