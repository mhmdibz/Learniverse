namespace Learniverse.Application.Interfaces.Identity;

public sealed record RegisterResult
{
    public bool Succeeded { get; }
    public string? UserId { get; }
    public IReadOnlyList<string> Errors { get; }

    private RegisterResult(
        bool succeeded,
        string? userId,
        IReadOnlyList<string> errors)
    {
        Succeeded = succeeded;
        UserId = userId;
        Errors = errors;
    }

    public static RegisterResult Success(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        return new(
            succeeded: true,
            userId: userId,
            errors: Array.Empty<string>());
    }

    public static RegisterResult Failure(IReadOnlyList<string> errors)
    {
        if (errors is null || errors.Count == 0)
        {
            throw new ArgumentException(
                "Failure result must contain at least one error.",
                nameof(errors));
        }

        return new(
            succeeded: false,
            userId: null,
            errors: errors);
    }
}