using Learniverse.Application.Common.DTOs;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enums;
using Learniverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Learniverse.Persistence.Repositories;

public class SectionRepository : ISectionRepository
{
    private readonly AppDbContext _context;

    public SectionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Section?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Sections
            .Where(s => _context.Courses.Any(c =>
            c.Id == s.CourseId &&
            c.Status == CourseStatus.Published))
            .FirstOrDefaultAsync(
                s => s.Id == id,
                cancellationToken);
    }
    public async Task<IReadOnlyList<SectionListDto>> GetAllAsync(
    bool isAdmin,
    string? instructorId,
    CancellationToken cancellationToken)
    {
        return await _context.Sections
            .AsNoTracking()
            .Where(s =>
                isAdmin ||
                _context.Courses.Any(c =>
                    c.Id == s.CourseId &&
                    (c.Status == CourseStatus.Published ||
                     (instructorId != null &&
                      c.InstructorId == instructorId))))
            .OrderBy(s => s.CourseId)
            .ThenBy(s => s.Order)
            .Select(s => new SectionListDto(
                s.Id,
                s.Title,
                s.Description,
                s.Order,
                s.CourseId))
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<SectionListDto>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        return await _context.Sections.AsNoTracking().Where(s => s.CourseId == courseId)
            .Where(s => _context.Courses.Any(c =>
            c.Id == s.CourseId &&
            c.Status == CourseStatus.Published))
            .OrderBy(s => s.Order).Select(s => new SectionListDto(s.Id,
            s.Title,
            s.Description,
            s.Order,
            s.CourseId))
            .ToListAsync(cancellationToken);
    }
    public async Task<Section?> GetByIdWithLessonsAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return await _context.Sections
            .Include(s => s.Lessons)
            .FirstOrDefaultAsync(
                s => s.Id == id,
                cancellationToken);
    }
    public async Task<IReadOnlyList<SectionListDto>>
    GetByCourseIdIncludingUnpublishedAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        return await _context.Sections
            .AsNoTracking()
            .Where(s => s.CourseId == courseId)
            .OrderBy(s => s.Order)
            .Select(s => new SectionListDto(
                s.Id,
                s.Title,
                s.Description,
                s.Order,
                s.CourseId))
            .ToListAsync(cancellationToken);
    }
    public async Task<Section?> GetByIdIncludingUnpublishedAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return await _context.Sections
            .FirstOrDefaultAsync(
                s => s.Id == id,
                cancellationToken);
    }
}