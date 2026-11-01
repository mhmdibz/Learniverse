using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Learniverse.Application.Interfaces.Payments;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Learniverse.Infrastructure.Payments;

public sealed class PaymobPaymentGateway : IPaymentGateway
{
    private readonly HttpClient _httpClient;
    private readonly PaymobOptions _options;
    private readonly ILogger<PaymobPaymentGateway> _logger;

    public PaymobPaymentGateway(
        HttpClient httpClient,
        IOptions<PaymobOptions> options,
        ILogger<PaymobPaymentGateway> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        _httpClient.BaseAddress =
            new Uri(_options.ApiBaseUrl, UriKind.Absolute);
    }

    public async Task<PaymentSessionResult> StartPaymentAsync(
        StartPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var specialReference = request.PaymentId.ToString("N");

        var payload = new Dictionary<string, object>
        {
            ["amount"] = request.AmountInMinorUnits,
            ["currency"] = request.Currency,
            ["payment_methods"] = _options.IntegrationIds,
            ["items"] = new[]
            {
                new
                {
                    name = request.ItemName,
                    amount = request.AmountInMinorUnits,
                    description = request.ItemName,
                    quantity = 1
                }
            },
            ["billing_data"] = new
            {
                first_name = request.Customer.FirstName,
                last_name = request.Customer.LastName,
                phone_number = request.Customer.PhoneNumber
            },
            ["special_reference"] = specialReference,
            ["expiration"] = _options.ExpirationSeconds
        };

        if (!string.IsNullOrWhiteSpace(_options.NotificationUrl))
            payload["notification_url"] = _options.NotificationUrl;

        if (!string.IsNullOrWhiteSpace(_options.RedirectionUrl))
            payload["redirection_url"] = _options.RedirectionUrl;

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "v1/intention/");

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Token", _options.SecretKey);

        httpRequest.Content = JsonContent.Create(payload);

        try
        {
            using var response = await _httpClient.SendAsync(
                httpRequest,
                cancellationToken);

            if ((int)response.StatusCode is >= 400 and < 500)
            {
                var rejectionBody = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                System.Diagnostics.Debug.WriteLine(
                    $"Paymob rejection ({(int)response.StatusCode}): {rejectionBody}");

                return new PaymentSessionResult(
                    PaymentSessionStatus.Rejected,
                    null,
                    null);
            }

            if (!response.IsSuccessStatusCode)
            {
                return new PaymentSessionResult(
                    PaymentSessionStatus.Unknown,
                    null,
                    null);
            }

            var responseBody = await response.Content.ReadAsStringAsync(
                cancellationToken);

            using var document = JsonDocument.Parse(responseBody);

            if (!document.RootElement.TryGetProperty(
                    "client_secret",
                    out var clientSecretElement) ||
                clientSecretElement.ValueKind != JsonValueKind.String)
            {
                return new PaymentSessionResult(
                    PaymentSessionStatus.Unknown,
                    null,
                    null);
            }

            var clientSecret = clientSecretElement.GetString();

            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                return new PaymentSessionResult(
                    PaymentSessionStatus.Unknown,
                    null,
                    null);
            }

            var checkoutUrl = BuildCheckoutUrl(clientSecret);

            return new PaymentSessionResult(
                PaymentSessionStatus.Accepted,
                checkoutUrl,
                specialReference);
        }
        catch (HttpRequestException)
        {
            return new PaymentSessionResult(
                PaymentSessionStatus.Unknown,
                null,
                null);
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return new PaymentSessionResult(
                PaymentSessionStatus.Unknown,
                null,
                null);
        }
        catch (JsonException)
        {
            return new PaymentSessionResult(
                PaymentSessionStatus.Unknown,
                null,
                null);
        }
    }

    public Task<PaymentNotificationResult> VerifyNotificationAsync(
        PaymentNotificationRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            using var document = JsonDocument.Parse(request.RawBody);
            var root = document.RootElement;

            if (!root.TryGetProperty("type", out var typeElement) ||
                typeElement.GetString() != "TRANSACTION" ||
                !root.TryGetProperty("obj", out var transaction))
            {
                return Task.FromResult(UnverifiedNotification());
            }

            if (!TryBuildHmacInput(transaction, out var hmacInput))
                return Task.FromResult(UnverifiedNotification());

            if (string.IsNullOrWhiteSpace(_options.HmacSecret) ||
                string.IsNullOrWhiteSpace(request.Signature))
            {
                return Task.FromResult(UnverifiedNotification());
            }

            var expectedSignature = HMACSHA512.HashData(
                Encoding.UTF8.GetBytes(_options.HmacSecret),
                Encoding.UTF8.GetBytes(hmacInput));

            byte[] receivedSignature;

            try
            {
                receivedSignature =
                    Convert.FromHexString(request.Signature.Trim());
            }
            catch (FormatException)
            {
                return Task.FromResult(UnverifiedNotification());
            }

            if (!CryptographicOperations.FixedTimeEquals(
                    expectedSignature,
                    receivedSignature))
            {
                return Task.FromResult(UnverifiedNotification());
            }

            if (!TryGetValueAtPath(
                    transaction,
                    ["amount_cents"],
                    out var amountText) ||
                !long.TryParse(
                    amountText,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var amount) ||
                !TryGetValueAtPath(
                    transaction,
                    ["currency"],
                    out var currency) ||
                !TryGetValueAtPath(
                    transaction,
                    ["id"],
                    out var transactionId) ||
                !TryGetBoolean(transaction, "success", out var success) ||
                !TryGetBoolean(transaction, "pending", out var pending))
            {
                return Task.FromResult(UnverifiedNotification());
            }

            var paymentReference =
                TryGetValueAtPath(
                    transaction,
                    ["order", "merchant_order_id"],
                    out var nestedReference)
                    ? nestedReference
                    : TryGetValueAtPath(
                        transaction,
                        ["merchant_order_id"],
                        out var directReference)
                        ? directReference
                        : null;

            var status = IsVoided(transaction)
                ? PaymentNotificationStatus.Unrecognized
                : IsRefunded(transaction)
                    ? PaymentNotificationStatus.Refunded
                    : pending
                        ? PaymentNotificationStatus.Pending
                        : success
                            ? PaymentNotificationStatus.Succeeded
                            : PaymentNotificationStatus.Failed;

            return Task.FromResult(
                new PaymentNotificationResult(
                    true,
                    paymentReference,
                    amount,
                    currency,
                    transactionId,
                    status));
        }
        catch (JsonException)
        {
            return Task.FromResult(UnverifiedNotification());
        }
    }

    public async Task<PaymentRefundResult> RefundAsync(
        PaymentRefundRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(_options.SecretKey))
        {
            _logger.LogError(
                "Paymob refund was not sent because the SecretKey is missing.");
            return new PaymentRefundResult(PaymentRefundStatus.Rejected);
        }

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _logger.LogError(
                "Paymob refund was not sent because the ApiKey is missing; refund verification would not be possible.");
            return new PaymentRefundResult(PaymentRefundStatus.Rejected);
        }

        if (!long.TryParse(
                request.ProviderTransactionId,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var transactionId) ||
            transactionId <= 0)
        {
            _logger.LogError(
                "Paymob refund was not sent because the provider transaction ID is invalid.");
            return new PaymentRefundResult(PaymentRefundStatus.Rejected);
        }

        if (request.AmountInMinorUnits <= 0)
        {
            _logger.LogError(
                "Paymob refund was not sent because the refund amount is not positive.");
            return new PaymentRefundResult(PaymentRefundStatus.Rejected);
        }

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "api/acceptance/void_refund/refund");

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Token", _options.SecretKey);
        httpRequest.Content = JsonContent.Create(new
        {
            transaction_id = transactionId,
            amount_cents = request.AmountInMinorUnits
        });

        try
        {
            using var response = await _httpClient.SendAsync(
                httpRequest,
                cancellationToken);

            if ((int)response.StatusCode is >= 400 and < 500)
            {
                var providerMessage = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                _logger.LogWarning(
                    "Paymob rejected the refund request with HTTP {StatusCode}. Provider response: {ProviderResponse}",
                    (int)response.StatusCode,
                    GetProviderErrorMessage(providerMessage));

                return new PaymentRefundResult(PaymentRefundStatus.Rejected);
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Paymob refund request returned HTTP {StatusCode}; the outcome is treated as unknown.",
                    (int)response.StatusCode);

                return new PaymentRefundResult(PaymentRefundStatus.Unknown);
            }

            return new PaymentRefundResult(PaymentRefundStatus.Accepted);
        }
        catch (HttpRequestException)
        {
            _logger.LogWarning(
                "The Paymob refund request failed before a definitive response was received.");
            return new PaymentRefundResult(PaymentRefundStatus.Unknown);
        }
        catch (OperationCanceledException)
        {
            // The request may have reached Paymob even if the caller disconnected.
            _logger.LogWarning(
                "The Paymob refund request was cancelled after it may have reached the provider.");
            return new PaymentRefundResult(PaymentRefundStatus.Unknown);
        }
    }

    private static string GetProviderErrorMessage(string responseBody)
    {
        try
        {
            using var document = JsonDocument.Parse(responseBody);

            foreach (var propertyName in new[] { "message", "detail", "error" })
            {
                if (document.RootElement.TryGetProperty(
                        propertyName,
                        out var property) &&
                    property.ValueKind == JsonValueKind.String)
                {
                    var message = property.GetString();

                    if (!string.IsNullOrWhiteSpace(message))
                        return message.Length <= 500
                            ? message
                            : message[..500];
                }
            }
        }
        catch (JsonException)
        {
            // Do not log an unstructured provider response that may contain sensitive data.
        }

        return "No safe provider error message was returned.";
    }

    public async Task<PaymentRefundVerificationResult> VerifyRefundAsync(
        string providerTransactionId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey) ||
            !long.TryParse(
                providerTransactionId,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var expectedTransactionId) ||
            expectedTransactionId <= 0)
        {
            return UnverifiedRefund();
        }

        try
        {
            using var tokenRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "api/auth/tokens")
            {
                Content = JsonContent.Create(new { api_key = _options.ApiKey })
            };

            using var tokenResponse = await _httpClient.SendAsync(
                tokenRequest,
                cancellationToken);

            if (!tokenResponse.IsSuccessStatusCode)
                return UnverifiedRefund();

            using var tokenDocument = JsonDocument.Parse(
                await tokenResponse.Content.ReadAsStringAsync(
                    cancellationToken));

            if (!tokenDocument.RootElement.TryGetProperty(
                    "token",
                    out var tokenElement) ||
                tokenElement.ValueKind != JsonValueKind.String ||
                string.IsNullOrWhiteSpace(tokenElement.GetString()))
            {
                return UnverifiedRefund();
            }

            using var transactionRequest = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/acceptance/transactions/{expectedTransactionId}");
            transactionRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", tokenElement.GetString());

            using var transactionResponse = await _httpClient.SendAsync(
                transactionRequest,
                cancellationToken);

            if (!transactionResponse.IsSuccessStatusCode)
                return UnverifiedRefund();

            using var transactionDocument = JsonDocument.Parse(
                await transactionResponse.Content.ReadAsStringAsync(
                    cancellationToken));

            var transaction = transactionDocument.RootElement;

            if (!TryGetValueAtPath(transaction, ["id"], out var idText) ||
                !long.TryParse(
                    idText,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var transactionId) ||
                transactionId != expectedTransactionId ||
                !TryGetBoolean(transaction, "is_refunded", out var isRefunded) ||
                !TryGetValueAtPath(transaction, ["currency"], out var currency))
            {
                return UnverifiedRefund();
            }

            long? refundedAmount = null;

            if (TryGetValueAtPath(
                    transaction,
                    ["refunded_amount_cents"],
                    out var refundedAmountText) &&
                long.TryParse(
                    refundedAmountText,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var parsedRefundedAmount))
            {
                refundedAmount = parsedRefundedAmount;
            }

            if (isRefunded && refundedAmount is null)
                return UnverifiedRefund();

            return new PaymentRefundVerificationResult(
                true,
                isRefunded,
                refundedAmount ?? 0,
                currency);
        }
        catch (HttpRequestException)
        {
            return UnverifiedRefund();
        }
        catch (JsonException)
        {
            return UnverifiedRefund();
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return UnverifiedRefund();
        }
    }

    private string BuildCheckoutUrl(string clientSecret)
    {
        var checkoutUri = new UriBuilder(_options.CheckoutBaseUrl)
        {
            Query =
                $"publicKey={Uri.EscapeDataString(_options.PublicKey)}" +
                $"&clientSecret={Uri.EscapeDataString(clientSecret)}"
        };

        return checkoutUri.Uri.ToString();
    }

    private static bool TryBuildHmacInput(
        JsonElement transaction,
        out string hmacInput)
    {
        var paths = new[]
        {
            new[] { "amount_cents" },
            new[] { "created_at" },
            new[] { "currency" },
            new[] { "error_occured" },
            new[] { "has_parent_transaction" },
            new[] { "id" },
            new[] { "integration_id" },
            new[] { "is_3d_secure" },
            new[] { "is_auth" },
            new[] { "is_capture" },
            new[] { "is_refunded" },
            new[] { "is_standalone_payment" },
            new[] { "is_voided" },
            new[] { "order", "id" },
            new[] { "owner" },
            new[] { "pending" },
            new[] { "source_data", "pan" },
            new[] { "source_data", "sub_type" },
            new[] { "source_data", "type" },
            new[] { "success" }
        };

        var builder = new StringBuilder();

        foreach (var path in paths)
        {
            if (!TryGetValueAtPath(transaction, path, out var value))
            {
                hmacInput = string.Empty;
                return false;
            }

            builder.Append(value);
        }

        hmacInput = builder.ToString();
        return true;
    }

    private static bool TryGetValueAtPath(
        JsonElement element,
        string[] path,
        out string value)
    {
        foreach (var segment in path)
        {
            if (element.ValueKind != JsonValueKind.Object ||
                !element.TryGetProperty(segment, out element))
            {
                value = string.Empty;
                return false;
            }
        }

        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                value = element.GetString() ?? string.Empty;
                return true;

            case JsonValueKind.Number:
                value = element.GetRawText();
                return true;

            case JsonValueKind.True:
                value = "true";
                return true;

            case JsonValueKind.False:
                value = "false";
                return true;

            default:
                value = string.Empty;
                return false;
        }
    }

    private static bool TryGetBoolean(
        JsonElement element,
        string propertyName,
        out bool value)
    {
        value = false;

        if (element.ValueKind != JsonValueKind.Object ||
            !element.TryGetProperty(propertyName, out var property))
        {
            return false;
        }

        if (property.ValueKind == JsonValueKind.True)
        {
            value = true;
            return true;
        }

        if (property.ValueKind == JsonValueKind.False)
            return true;

        return false;
    }

    private static bool IsRefunded(JsonElement transaction)
    {
        return TryGetBoolean(transaction, "is_refunded", out var isRefunded) &&
               isRefunded;
    }

    private static bool IsVoided(JsonElement transaction)
    {
        return TryGetBoolean(transaction, "is_voided", out var isVoided) &&
               isVoided;
    }

    private static PaymentRefundVerificationResult UnverifiedRefund()
    {
        return new PaymentRefundVerificationResult(
            false,
            false,
            null,
            null);
    }

    private static PaymentNotificationResult UnverifiedNotification()
    {
        return new PaymentNotificationResult(
            false,
            null,
            null,
            null,
            null,
            PaymentNotificationStatus.Unrecognized);
    }
}
