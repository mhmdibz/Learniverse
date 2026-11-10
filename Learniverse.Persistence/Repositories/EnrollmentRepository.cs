using Learniverse.Application.Common.DTOs;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enum;
using Learniverse.Domain.Enums;
using Learniverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Learniverse.Persistence.Repositories;

public sealed class EnrollmentRepository : IEnrollmentRepository
{
    private readonly AppDbContext _context;

    public EnrollmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> HasAccessAsync(
        string studentId,
        Guid courseId,
        CancellationToken cancellationToken)
    {
        return _context.Enrollments.AnyAsync(
            e => e.StudentId == studentId
                 && e.CourseId == courseId
                 && (e.Status == EnrollmentStatus.Active
                     || e.Status == EnrollmentStatus.Completed),
            cancellationToken);
    }
    public Task<Enrollment?> GetByStudentAndCourseAsync(
    string studentId,
    Guid courseId,
    CancellationToken cancellationToken)
    {
        return _context.Enrollments.FirstOrDefaultAsync(
            enrollment =>
                enrollment.StudentId == studentId &&
                enrollment.CourseId == courseId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<StudentEnrollmentDto>> GetStudentEnrollmentsAsync(
        string studentId,
        CancellationToken cancellationToken)
    {
        return await (
            from enrollment in _context.Enrollments.AsNoTracking()
            join course in _context.Courses.AsNoTracking()
                on enrollment.CourseId equals course.Id
            where enrollment.StudentId == studentId
                  && (enrollment.Status == EnrollmentStatus.Active
                      || enrollment.Status == EnrollmentStatus.Completed)
            orderby enrollment.EnrolledAtUtc descending
            select new StudentEnrollmentDto(
                course.Id,
                course.Title,
                course.Price,
                course.Status,
                enrollment.Status,
                enrollment.EnrolledAtUtc,
                enrollment.CompletedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Enrollment enrollment,
        CancellationToken cancellationToken)
    {
        await _context.Enrollments.AddAsync(enrollment, cancellationToken);
    }
}