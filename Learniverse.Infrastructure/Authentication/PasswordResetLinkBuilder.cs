using System.Text;
using Learniverse.Application.Interfaces.Identity;
using Microsoft.Extensions.Options;

namespace Learniverse.Infrastructure.Authentication;

public sealed class PasswordResetLinkBuilder
    : IPasswordResetLinkBuilder
{
    private readonly PasswordResetLinkOptions _options;

    public PasswordResetLinkBuilder(
        IOptions<PasswordResetLinkOptions> options)
    {
        _options = options.Value;
    }

    public string Build(string email, string token)
    {
        var encodedEmail = Uri.EscapeDataString(email);
        var encodedToken = EncodeBase64Url(token);

        return $"{_options.ResetPasswordUrl}#email={encodedEmail}&token={encodedToken}";
    }

    private static string EncodeBase64Url(string value)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(value))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
