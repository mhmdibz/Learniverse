namespace Learniverse.Infrastructure.Authentication;

public sealed class PasswordResetLinkOptions
{
    public const string SectionName = "PasswordReset";

    public string ResetPasswordUrl { get; set; } = string.Empty;
}