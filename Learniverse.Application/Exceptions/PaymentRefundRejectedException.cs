namespace Learniverse.Application.Exceptions;

public sealed class PaymentRefundRejectedException : Exception
{
    public PaymentRefundRejectedException()
        : base("The refund request was not accepted. Check the provider response and settings before retrying.")
    {
    }
}
