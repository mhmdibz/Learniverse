using System.Text;
using Microsoft.Extensions.Options;

namespace Learniverse.Infrastructure.Authentication;

public sealed class JwtOptionsValidator
    : IValidateOptions<JwtOptions>
{
    public ValidateOptionsResult Validate(
        string? name,
        JwtOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.SecretKey))
        {
            failures.Add("JWT SecretKey is required.");
        }
        else if (Encoding.UTF8.GetByteCount(options.SecretKey) < 32)
        {
            failures.Add(
                "JWT SecretKey must be at least 32 UTF-8 bytes.");
        }

        if (string.IsNullOrWhiteSpace(options.Issuer))
        {
            failures.Add("JWT Issuer is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            failures.Add("JWT Audience is required.");
        }

        if (options.ExpirationMinutes <= 0)
        {
            failures.Add(
                "JWT ExpirationMinutes must be greater than zero.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}