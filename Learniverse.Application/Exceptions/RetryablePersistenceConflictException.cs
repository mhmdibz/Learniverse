namespace Learniverse.Application.Exceptions;

public sealed class RetryablePersistenceConflictException : Exception
{
    public RetryablePersistenceConflictException(Exception innerException)
        : base(
            "A persistence conflict occurred. The operation can be retried.",
            innerException)
    {
    }
}
