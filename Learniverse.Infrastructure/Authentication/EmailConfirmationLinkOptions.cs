namespace Learniverse.Infrastructure.Authentication;

public sealed class EmailConfirmationLinkOptions
{
    public const string SectionName = "EmailConfirmation";

    public string ConfirmationUrl { get; set; } = string.Empty;
}
