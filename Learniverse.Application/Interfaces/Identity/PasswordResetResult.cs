namespace Learniverse.Application.Interfaces.Identity;

public sealed record PasswordResetResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static PasswordResetResult Success() =>
        new(true, Array.Empty<string>());

    public static PasswordResetResult Failure(IEnumerable<string> errors) =>
        new(false, errors.ToArray());
}
