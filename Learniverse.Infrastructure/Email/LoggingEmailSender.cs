using Learniverse.Application.Interfaces.Email;
using Microsoft.Extensions.Logging;

namespace Learniverse.Infrastructure.Email;

public sealed class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(
        EmailMessage message,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation(
            "Email requested. Purpose: {EmailPurpose}; recipient: {MaskedRecipient}.",
            message.Purpose,
            MaskEmail(message.ToEmail));

        return Task.CompletedTask;
    }

    private static string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');

        if (atIndex <= 0 || atIndex == email.Length - 1)
        {
            return "***";
        }

        var localPart = email[..atIndex];
        var domain = email[(atIndex + 1)..];

        var maskedLocal = localPart.Length == 1
            ? "*"
            : $"{localPart[0]}***";

        var dotIndex = domain.LastIndexOf('.');
        var maskedDomain = dotIndex > 0
            ? $"{domain[0]}***{domain[dotIndex..]}"
            : $"{domain[0]}***";

        return $"{maskedLocal}@{maskedDomain}";
    }
}
