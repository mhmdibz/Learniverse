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

    public async Task AddAsync(
        Enrollment enrollment,
        CancellationToken cancellationToken)
    {
        await _context.Enrollments.AddAsync(enrollment, cancellationToken);
    }
}