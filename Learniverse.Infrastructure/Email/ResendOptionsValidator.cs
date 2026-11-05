using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Learniverse.Infrastructure.Email;

public sealed class ResendOptionsValidator
    : IValidateOptions<ResendOptions>
{
    public ValidateOptionsResult Validate(
        string? name,
        ResendOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            failures.Add("Resend:ApiKey is required.");
        }

        if (string.IsNullOrWhiteSpace(options.SenderName))
        {
            failures.Add("Resend:SenderName is required.");
        }

        if (string.IsNullOrWhiteSpace(options.SenderEmail))
        {
            failures.Add("Resend:SenderEmail is required.");
        }
        else
        {
            try
            {
                var senderAddress = new MailAddress(options.SenderEmail);

                if (!string.Equals(
                        senderAddress.Address,
                        options.SenderEmail,
                        StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add(
                        "Resend:SenderEmail must be a single email address.");
                }
            }
            catch (FormatException)
            {
                failures.Add(
                    "Resend:SenderEmail must be a valid email address.");
            }
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
