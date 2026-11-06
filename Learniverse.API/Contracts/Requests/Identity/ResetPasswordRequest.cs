namespace Learniverse.API.Contracts.Requests.Identity;

public sealed record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword);
