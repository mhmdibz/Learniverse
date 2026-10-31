namespace Learniverse.Application.Interfaces.Payments;

public interface IPaymentGateway
{
    Task<PaymentSessionResult> StartPaymentAsync(
        StartPaymentRequest request,
        CancellationToken cancellationToken);

    Task<PaymentNotificationResult> VerifyNotificationAsync(
        PaymentNotificationRequest request,
        CancellationToken cancellationToken);
}