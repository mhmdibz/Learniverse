namespace Learniverse.API.Contracts.Requests.Identity;

public sealed record RegisterRequest(
    string FullName,
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword
);