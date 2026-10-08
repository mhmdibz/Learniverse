using Learniverse.Application.Common.DTOs;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enums;
using Learniverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Learniverse.Persistence.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        return await _context.Courses
            .AnyAsync(
                c => c.Id == courseId,
                cancellationToken);
    }

    public async Task AddAsync(
        Course course,
        CancellationToken cancellationToken)
    {
        await _context.Courses.AddAsync(
            course,
            cancellationToken);
    }

    public async Task<Course?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Courses
            .Include(c => c.Category)
            .FirstOrDefaultAsync(
                c => c.Id == id,
                cancellationToken);
    }
    public async Task<Course?> GetPublishedByIdAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return await _context.Courses
            .Include(c => c.Category)
            .Where(c => c.Status == CourseStatus.Published)
            .FirstOrDefaultAsync(
                c => c.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<CourseListDto>> GetAllAsync(
    bool isAdmin,
    string? instructorId,
    CancellationToken cancellationToken)
    {
        return await _context.Courses
            .AsNoTracking()
            .Where(c =>
                isAdmin ||
                c.Status == CourseStatus.Published ||
                (instructorId != null && c.InstructorId == instructorId))
            .OrderBy(c => c.Title)
            .Select(c => new CourseListDto(
                c.Id,
                c.Title,
                c.Price,
                c.Status,
                c.InstructorId,
                c.CategoryId,
                c.Category.Name,
                c.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<Course?> GetByIdWithSectionsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Courses
            .Include(c => c.Sections)
            .ThenInclude(s => s.Lessons)
            .FirstOrDefaultAsync(
                c => c.Id == id,
                cancellationToken);
    }
}
