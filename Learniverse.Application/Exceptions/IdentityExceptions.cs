namespace Learniverse.Application.Exceptions;

public sealed class AccountLockedException : Exception
{
    public AccountLockedException()
        : base("Account is temporarily locked.")
    {
    }
}

public sealed class AccountNotAllowedException : Exception
{
    public AccountNotAllowedException()
        : base("Account is not allowed to sign in.")
    {
    }
}