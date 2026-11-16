namespace Learniverse.Application.Interfaces.Identity;

public interface IIdentityService
{
    Task<string?> GetFullNameAsync(string userId);

    Task<RegisterResult> RegisterStudentAsync(
        string fullName,
        string userName,
        string email,
        string password,
        CancellationToken cancellationToken);
    Task<AuthenticationResult> AuthenticateAsync(
        string identifier,
        string password,
        CancellationToken cancellationToken);

    Task<string?> GeneratePasswordResetTokenAsync(
        string email,
        CancellationToken cancellationToken);

    Task<PasswordResetResult> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken);

    Task<string?> GenerateEmailConfirmationTokenAsync(
        string email,
        CancellationToken cancellationToken);

    Task<bool> ConfirmEmailAsync(
        string email,
        string token,
        CancellationToken cancellationToken);
}
