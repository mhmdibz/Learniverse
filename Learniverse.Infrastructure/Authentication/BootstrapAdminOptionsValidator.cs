using Microsoft.Extensions.Options;

namespace Learniverse.Infrastructure.Authentication;

public sealed class BootstrapAdminOptionsValidator
    : IValidateOptions<BootstrapAdminOptions>
{
    public ValidateOptionsResult Validate(
        string? name,
        BootstrapAdminOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Email))
        {
            errors.Add("Bootstrap admin email is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            errors.Add("Bootstrap admin password is required.");
        }

        if (options.Password.Length < 8)
        {
            errors.Add(
                "Bootstrap admin password must be at least 8 characters long.");
        }

        return errors.Count > 0
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}