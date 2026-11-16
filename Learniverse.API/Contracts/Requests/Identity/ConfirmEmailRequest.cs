namespace Learniverse.API.Contracts.Requests.Identity;

public sealed record ConfirmEmailRequest(string Email, string Token);
