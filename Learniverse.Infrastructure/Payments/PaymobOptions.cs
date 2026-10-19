namespace Learniverse.Infrastructure.Payments;

public sealed class PaymobOptions
{
    public const string SectionName = "Paymob";

    public string ApiBaseUrl { get; init; } =
        "https://accept.paymob.com/";

    public string CheckoutBaseUrl { get; init; } =
        "https://eg.checkout.paymob.com/";
    public string HmacSecret { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string PublicKey { get; init; } = string.Empty;

    public int[] IntegrationIds { get; init; } = [];

    public string NotificationUrl { get; init; } = string.Empty;
    public string RedirectionUrl { get; init; } = string.Empty;

    public int ExpirationSeconds { get; init; } = 1800;
}
