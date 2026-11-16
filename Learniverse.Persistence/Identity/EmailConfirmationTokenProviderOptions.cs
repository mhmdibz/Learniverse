using Microsoft.AspNetCore.Identity;

namespace Learniverse.Persistence.Identity;

public sealed class EmailConfirmationTokenProviderOptions
    : DataProtectionTokenProviderOptions
{
    public EmailConfirmationTokenProviderOptions()
    {
        Name = "EmailConfirmation";
        TokenLifespan = TimeSpan.FromHours(24);
    }
}
