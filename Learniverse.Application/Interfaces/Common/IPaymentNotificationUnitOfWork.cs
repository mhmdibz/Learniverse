namespace Learniverse.Application.Interfaces.Common;

public interface IPaymentNotificationUnitOfWork : IUnitOfWork
{
    Task<int> SavePaymentNotificationChangesAsync(
        CancellationToken cancellationToken);
}
