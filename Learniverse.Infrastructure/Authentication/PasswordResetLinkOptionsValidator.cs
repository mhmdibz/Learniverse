using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Learniverse.Infrastructure.Authentication;

public sealed class PasswordResetLinkOptionsValidator
    : IValidateOptions<PasswordResetLinkOptions>
{
    private readonly IHostEnvironment _environment;

    public PasswordResetLinkOptionsValidator(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public ValidateOptionsResult Validate(
        string? name,
        PasswordResetLinkOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ResetPasswordUrl) ||
            !Uri.TryCreate(
                options.ResetPasswordUrl,
                UriKind.Absolute,
                out var uri))
        {
            return ValidateOptionsResult.Fail(
                "PasswordReset:ResetPasswordUrl must be an absolute URL.");
        }

        if (uri.Scheme != Uri.UriSchemeHttp &&
            uri.Scheme != Uri.UriSchemeHttps)
        {
            return ValidateOptionsResult.Fail(
                "PasswordReset:ResetPasswordUrl must use HTTP or HTTPS.");
        }

        if (!_environment.IsDevelopment() &&
            uri.Scheme != Uri.UriSchemeHttps)
        {
            return ValidateOptionsResult.Fail(
                "PasswordReset:ResetPasswordUrl must use HTTPS outside Development.");
        }

        if (!string.IsNullOrEmpty(uri.Fragment))
        {
            return ValidateOptionsResult.Fail(
                "PasswordReset:ResetPasswordUrl must not include a fragment.");
        }

        return ValidateOptionsResult.Success;
    }
}
