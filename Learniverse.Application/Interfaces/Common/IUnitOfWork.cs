namespace Learniverse.Application.Interfaces.Common;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task BeginTransactionAsync(
        CancellationToken cancellationToken,
        System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted);

    Task CommitTransactionAsync(CancellationToken cancellationToken);

    Task RollbackTransactionAsync(CancellationToken cancellationToken);
}
