namespace Learniverse.Application.Interfaces.Identity;

public interface IIdentityService
{
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
}