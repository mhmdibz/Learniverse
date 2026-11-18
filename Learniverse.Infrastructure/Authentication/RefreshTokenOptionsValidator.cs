using Microsoft.Extensions.Options;

namespace Learniverse.Infrastructure.Authentication;

public sealed class RefreshTokenOptionsValidator
    : IValidateOptions<RefreshTokenOptions>
{
    public ValidateOptionsResult Validate(
        string? name,
        RefreshTokenOptions options)
    {
        if (options.InactivityExpirationDays <= 0)
        {
            return ValidateOptionsResult.Fail(
                "RefreshToken:InactivityExpirationDays must be greater than zero.");
        }

        if (options.MaximumSessionAgeDays <= 0)
        {
            return ValidateOptionsResult.Fail(
                "RefreshToken:MaximumSessionAgeDays must be greater than zero.");
        }

        if (options.InactivityExpirationDays >
            options.MaximumSessionAgeDays)
        {
            return ValidateOptionsResult.Fail(
                "RefreshToken:InactivityExpirationDays cannot exceed RefreshToken:MaximumSessionAgeDays.");
        }

        return ValidateOptionsResult.Success;
    }
}
