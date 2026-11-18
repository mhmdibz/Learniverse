namespace Learniverse.Infrastructure.Authentication;

public sealed class RefreshTokenOptions
{
    public const string SectionName = "RefreshToken";

    public int InactivityExpirationDays { get; set; } = 14;

    public int MaximumSessionAgeDays { get; set; } = 30;
}
