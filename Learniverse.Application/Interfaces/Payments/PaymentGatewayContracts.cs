namespace Learniverse.Application.Interfaces.Payments;

public sealed record StartPaymentRequest(
    Guid PaymentId,
    long AmountInMinorUnits,
    string Currency,
    string ItemName,
    PaymentCustomerDetails Customer);

public sealed record PaymentRefundRequest(
    string ProviderTransactionId,
    long AmountInMinorUnits);

public enum PaymentRefundStatus
{
    Accepted,
    Rejected,
    Unknown
}

public sealed record PaymentRefundResult(PaymentRefundStatus Status);

public sealed record PaymentRefundVerificationResult(
    bool IsVerified,
    bool IsRefunded,
    long? RefundedAmountInMinorUnits,
    string? Currency);

public sealed record PaymentCustomerDetails(
    string FirstName,
    string LastName,
    string PhoneNumber);

public enum PaymentSessionStatus
{
    Accepted,
    Rejected,
    Unknown
}

public sealed record PaymentSessionResult(
    PaymentSessionStatus Status,
    string? CheckoutUrl,
    string? ExternalReference);

public sealed record PaymentNotificationRequest(
    string RawBody,
    string Signature);

public enum PaymentNotificationStatus
{
    Succeeded,
    Failed,
    Pending,
    Refunded,
    Unrecognized
}

public sealed record PaymentNotificationResult(
    bool IsVerified,
    string? PaymentReference,
    long? AmountInMinorUnits,
    string? Currency,
    string? ProviderTransactionId,
    PaymentNotificationStatus Status);
