using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Learniverse.Persistence.Identity;

public sealed class EmailConfirmationTokenProvider
    : DataProtectorTokenProvider<ApplicationUser>
{
    public EmailConfirmationTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<EmailConfirmationTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<ApplicationUser>> logger)
        : base(dataProtectionProvider, options, logger)
    {
    }
}
