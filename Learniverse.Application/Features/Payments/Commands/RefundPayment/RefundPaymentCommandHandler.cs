using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Payments;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enum;
using MediatR;

namespace Learniverse.Application.Features.Payments.Commands.RefundPayment;

public sealed class RefundPaymentCommandHandler
    : IRequestHandler<RefundPaymentCommand, RefundPaymentResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IUnitOfWork _unitOfWork;

    public RefundPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _unitOfWork = unitOfWork;
    }

    public async Task<RefundPaymentResponse> Handle(
        RefundPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(
            request.PaymentId,
            cancellationToken);

        if (payment is null)
            throw new NotFoundException(nameof(Payment), request.PaymentId);

        if (payment.Status != PaymentStatus.Paid)
            throw new ConflictException(
                "Only a paid payment can be refunded.");

        if (string.IsNullOrWhiteSpace(payment.ProviderTransactionId))
            throw new ConflictException(
                "The provider transaction ID is missing for this payment.");

        if (payment.RefundStatus is RefundStatus.Pending
            or RefundStatus.OutcomeUnknown
            or RefundStatus.Completed)
        {
            throw new ConflictException(
                "A refund request is already active, unresolved, or completed.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            payment.MarkRefundAsPending();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }

        PaymentRefundResult gatewayResult;

        try
        {
            gatewayResult = await _paymentGateway.RefundAsync(
                new PaymentRefundRequest(
                    payment.ProviderTransactionId,
                    decimal.ToInt64(payment.Amount * 100m)),
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            payment.MarkRefundOutcomeUnknown();
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            throw new PaymentRefundOutcomeUnknownException();
        }

        if (gatewayResult.Status == PaymentRefundStatus.Rejected)
        {
            payment.MarkRefundAsRejected();
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            throw new PaymentRefundRejectedException();
        }

        if (gatewayResult.Status == PaymentRefundStatus.Unknown)
        {
            payment.MarkRefundOutcomeUnknown();
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            throw new PaymentRefundOutcomeUnknownException();
        }

        return new RefundPaymentResponse(
            payment.Id,
            payment.RefundStatus.ToString());
    }
}
