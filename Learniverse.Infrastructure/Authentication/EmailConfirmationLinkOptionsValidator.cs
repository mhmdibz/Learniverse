using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Learniverse.Infrastructure.Authentication;

public sealed class EmailConfirmationLinkOptionsValidator
    : IValidateOptions<EmailConfirmationLinkOptions>
{
    private readonly IHostEnvironment _environment;

    public EmailConfirmationLinkOptionsValidator(
        IHostEnvironment environment)
    {
        _environment = environment;
    }

    public ValidateOptionsResult Validate(
        string? name,
        EmailConfirmationLinkOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ConfirmationUrl) ||
            !Uri.TryCreate(
                options.ConfirmationUrl,
                UriKind.Absolute,
                out var uri))
        {
            return ValidateOptionsResult.Fail(
                "EmailConfirmation:ConfirmationUrl must be an absolute URL.");
        }

        if (uri.Scheme != Uri.UriSchemeHttp &&
            uri.Scheme != Uri.UriSchemeHttps)
        {
            return ValidateOptionsResult.Fail(
                "EmailConfirmation:ConfirmationUrl must use HTTP or HTTPS.");
        }

        if (!_environment.IsDevelopment() &&
            uri.Scheme != Uri.UriSchemeHttps)
        {
            return ValidateOptionsResult.Fail(
                "EmailConfirmation:ConfirmationUrl must use HTTPS outside Development.");
        }

        if (!string.IsNullOrEmpty(uri.Fragment))
        {
            return ValidateOptionsResult.Fail(
                "EmailConfirmation:ConfirmationUrl must not include a fragment.");
        }

        return ValidateOptionsResult.Success;
    }
}
