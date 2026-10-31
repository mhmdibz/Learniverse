namespace Learniverse.Application.Interfaces.Payments;

public interface IPaymentGateway
{
    Task<PaymentSessionResult> StartPaymentAsync(
        StartPaymentRequest request,
        CancellationToken cancellationToken);

    Task<PaymentNotificationResult> VerifyNotificationAsync(
        PaymentNotificationRequest request,
        CancellationToken cancellationToken);

    Task<PaymentRefundResult> RefundAsync(
        PaymentRefundRequest request,
        CancellationToken cancellationToken);

    Task<PaymentRefundVerificationResult> VerifyRefundAsync(
        string providerTransactionId,
        CancellationToken cancellationToken);
}
