using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enum;
using Learniverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Learniverse.Persistence.Repositories;

public sealed class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Payment payment,
        CancellationToken cancellationToken)
    {
        await _context.Payments.AddAsync(payment, cancellationToken);
    }

    public Task<Payment?> GetByIdAsync(
        Guid paymentId,
        CancellationToken cancellationToken)
    {
        return _context.Payments.FirstOrDefaultAsync(
            payment => payment.Id == paymentId,
            cancellationToken);
    }

    public Task<Payment?> GetActivePendingAsync(
    string studentId,
    Guid courseId,
    DateTime utcNow,
    CancellationToken cancellationToken)
    {
        return _context.Payments
            .Where(payment =>
                payment.StudentId == studentId &&
                payment.CourseId == courseId &&
                payment.Status == PaymentStatus.Pending &&
                payment.ExpiresAtUtc > utcNow)
            .OrderByDescending(payment => payment.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int> ExpirePendingForStudentAndCourseAsync(
        string studentId,
        Guid courseId,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        return _context.Payments
            .Where(payment =>
                payment.StudentId == studentId &&
                payment.CourseId == courseId &&
                payment.Status == PaymentStatus.Pending &&
                payment.ExpiresAtUtc <= utcNow)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    payment => payment.Status,
                    PaymentStatus.Expired),
                cancellationToken);
    }

    public Task<Payment?> GetByExternalReferenceAsync(
    string externalReference,
    CancellationToken cancellationToken)
    {
        return _context.Payments.FirstOrDefaultAsync(
            payment => payment.ExternalReference == externalReference,
            cancellationToken);
    }

    public Task<int> ExpirePendingAsync(
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        return _context.Payments
            .Where(payment =>
                payment.Status == PaymentStatus.Pending &&
                payment.ExpiresAtUtc <= utcNow)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    payment => payment.Status,
                    PaymentStatus.Expired),
                cancellationToken);
    }
}
