using Learniverse.Domain.Entities;

namespace Learniverse.Application.Interfaces.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(
        Payment payment,
        CancellationToken cancellationToken);

    Task<Payment?> GetByIdAsync(
        Guid paymentId,
        CancellationToken cancellationToken);

    Task<Payment?> GetActivePendingAsync(
    string studentId,
    Guid courseId,
    DateTime utcNow,
    CancellationToken cancellationToken);
}