namespace Learniverse.Application.Interfaces.Identity;

public sealed record AuthenticationResult
{
    public bool Succeeded { get; }
    public AuthenticatedUser? User { get; }
    public AuthenticationFailureReason? FailureReason { get; }

    private AuthenticationResult(
        bool succeeded,
        AuthenticatedUser? user,
        AuthenticationFailureReason? failureReason)
    {
        Succeeded = succeeded;
        User = user;
        FailureReason = failureReason;
    }

    public static AuthenticationResult Success(AuthenticatedUser user) =>
        new(true, user, null);

    public static AuthenticationResult Failure(AuthenticationFailureReason reason) =>
        new(false, null, reason);
}