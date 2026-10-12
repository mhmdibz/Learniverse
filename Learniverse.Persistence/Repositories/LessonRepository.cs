using Learniverse.Application.Common.DTOs;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enums;
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
     .Where(l => l.IsPreview &&
         _context.Sections.Any(s =>
             s.Id == l.SectionId &&
             _context.Courses.Any(c =>
                 c.Id == s.CourseId &&
                 c.Status == CourseStatus.Published)))
     .FirstOrDefaultAsync(
         l => l.Id == id,
         cancellationToken);
    }
    public async Task<IReadOnlyList<LessonListDto>> GetAllAsync(
     bool isAdmin,
     string? instructorId,
     CancellationToken cancellationToken)
    {
        var lessons =
            from lesson in _context.Lessons.AsNoTracking()
            join section in _context.Sections.AsNoTracking()
                on lesson.SectionId equals section.Id
            join course in _context.Courses.AsNoTracking()
                on section.CourseId equals course.Id
            where isAdmin ||
                  (instructorId != null &&
                   course.InstructorId == instructorId) ||
                  (lesson.IsPreview &&
                   course.Status == CourseStatus.Published)
            orderby course.Title,
                course.Id,
                section.Order,
                section.Id,
                lesson.Order
            select new LessonListDto(
                lesson.Id,
                lesson.Title,
                lesson.Description,
                lesson.Order,
                lesson.ContentType,
                lesson.SectionId,
                lesson.IsPreview);

        return await lessons.ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<LessonListDto>> GetAllByCourseIdAsync(
     Guid courseId,
     CancellationToken cancellationToken)
    {
        var lessons =
            from lesson in _context.Lessons.AsNoTracking()
            join section in _context.Sections.AsNoTracking()
                on lesson.SectionId equals section.Id
            where section.CourseId == courseId
            orderby section.Order, lesson.Order
            select new LessonListDto(
                lesson.Id,
                lesson.Title,
                lesson.Description,
                lesson.Order,
                lesson.ContentType,
                lesson.SectionId,
                lesson.IsPreview);

        return await lessons.ToListAsync(cancellationToken);
    }
    public async Task<Lesson?> GetByIdForCourseAsync(
    Guid courseId,
    Guid lessonId,
    CancellationToken cancellationToken)
    {
        return await _context.Lessons.FirstOrDefaultAsync(
            l => l.Id == lessonId &&
                 _context.Sections.Any(s =>
                     s.Id == l.SectionId &&
                     s.CourseId == courseId),
            cancellationToken);
    }
    public async Task<Lesson?> GetByIdIncludingUnpublishedAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return await _context.Lessons
            .FirstOrDefaultAsync(
                l => l.Id == id,
                cancellationToken);
    }
}