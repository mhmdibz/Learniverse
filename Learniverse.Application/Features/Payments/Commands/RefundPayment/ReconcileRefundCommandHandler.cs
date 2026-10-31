using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Payments;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enum;
using MediatR;

namespace Learniverse.Application.Features.Payments.Commands.RefundPayment;

public sealed class ReconcileRefundCommandHandler
    : IRequestHandler<ReconcileRefundCommand, ReconcileRefundResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IUnitOfWork _unitOfWork;

    public ReconcileRefundCommandHandler(
        IPaymentRepository paymentRepository,
        IEnrollmentRepository enrollmentRepository,
        IPaymentGateway paymentGateway,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _enrollmentRepository = enrollmentRepository;
        _paymentGateway = paymentGateway;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReconcileRefundResponse> Handle(
        ReconcileRefundCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(
            request.PaymentId,
            cancellationToken);

        if (payment is null)
            throw new NotFoundException(nameof(Payment), request.PaymentId);

        if (payment.RefundStatus == RefundStatus.Completed)
        {
            return CreateResponse(
                payment,
                isVerified: true,
                providerReportsRefund: true,
                refundedAmountInMinorUnits:
                    decimal.ToInt64(payment.Amount * 100m),
                currency: payment.Currency);
        }

        if (payment.Status != PaymentStatus.Paid ||
            payment.RefundStatus is RefundStatus.NotRequested
                or RefundStatus.Completed)
        {
            throw new ConflictException(
                "Only a paid payment with a refund request can be reconciled.");
        }

        if (string.IsNullOrWhiteSpace(payment.ProviderTransactionId))
            throw new ConflictException(
                "The provider transaction ID is missing for this payment.");

        var providerResult = await _paymentGateway.VerifyRefundAsync(
            payment.ProviderTransactionId,
            cancellationToken);

        if (!providerResult.IsVerified)
        {
            if (payment.RefundStatus == RefundStatus.Pending)
            {
                payment.MarkRefundOutcomeUnknown();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return CreateResponse(
                payment,
                isVerified: false,
                providerReportsRefund: false,
                refundedAmountInMinorUnits: null,
                currency: null);
        }

        if (!providerResult.IsRefunded)
        {
            return CreateResponse(
                payment,
                isVerified: true,
                providerReportsRefund: false,
                providerResult.RefundedAmountInMinorUnits,
                providerResult.Currency);
        }

        var expectedAmountInMinorUnits =
            decimal.ToInt64(payment.Amount * 100m);

        if (providerResult.RefundedAmountInMinorUnits !=
                expectedAmountInMinorUnits ||
            !string.Equals(
                providerResult.Currency,
                payment.Currency,
                StringComparison.OrdinalIgnoreCase))
        {
            if (payment.RefundStatus is RefundStatus.Pending
                or RefundStatus.Rejected)
            {
                payment.MarkRefundOutcomeUnknown();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return CreateResponse(
                payment,
                isVerified: true,
                providerReportsRefund: true,
                providerResult.RefundedAmountInMinorUnits,
                providerResult.Currency);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            payment.MarkAsRefunded();

            var enrollment =
                await _enrollmentRepository.GetByStudentAndCourseAsync(
                    payment.StudentId,
                    payment.CourseId,
                    cancellationToken);

            enrollment?.Cancel();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }

        return CreateResponse(
            payment,
            isVerified: true,
            providerReportsRefund: true,
            providerResult.RefundedAmountInMinorUnits,
            providerResult.Currency);
    }

    private static ReconcileRefundResponse CreateResponse(
        Payment payment,
        bool isVerified,
        bool providerReportsRefund,
        long? refundedAmountInMinorUnits,
        string? currency)
    {
        return new ReconcileRefundResponse(
            payment.Id,
            payment.RefundStatus.ToString(),
            isVerified,
            providerReportsRefund,
            refundedAmountInMinorUnits,
            currency);
    }
}
