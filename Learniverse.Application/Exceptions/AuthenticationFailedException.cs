namespace Learniverse.Application.Exceptions;

public sealed class AuthenticationFailedException : Exception
{
    public AuthenticationFailedException()
        : base("Authentication failed.")
    {
    }
}