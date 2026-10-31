using MediatR;

namespace Learniverse.Application.Features.Payments.Commands.RefundPayment;

public sealed record RefundPaymentCommand(Guid PaymentId)
    : IRequest<RefundPaymentResponse>;

public sealed record RefundPaymentResponse(
    Guid PaymentId,
    string RefundStatus);

public sealed record ReconcileRefundCommand(Guid PaymentId)
    : IRequest<ReconcileRefundResponse>;

public sealed record ReconcileRefundResponse(
    Guid PaymentId,
    string RefundStatus,
    bool IsVerified,
    bool ProviderReportsRefund,
    long? RefundedAmountInMinorUnits,
    string? Currency);
