namespace Learniverse.Application.Exceptions;

public sealed class PaymentRefundOutcomeUnknownException : Exception
{
    public PaymentRefundOutcomeUnknownException()
        : base("The refund outcome is unknown. Do not submit another refund request yet.")
    {
    }
}
