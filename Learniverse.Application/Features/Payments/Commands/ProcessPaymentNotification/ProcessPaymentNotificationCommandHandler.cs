using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Payments;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enum;
using Learniverse.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Learniverse.Application.Features.Payments.Commands.ProcessPaymentNotification;

public sealed class ProcessPaymentNotificationCommandHandler
    : IRequestHandler<
        ProcessPaymentNotificationCommand,
        ProcessPaymentNotificationResponse>
{
    private readonly IPaymentGateway _paymentGateway;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessPaymentNotificationCommandHandler> _logger;

    public ProcessPaymentNotificationCommandHandler(
        IPaymentGateway paymentGateway,
        IPaymentRepository paymentRepository,
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IUnitOfWork unitOfWork,
        ILogger<ProcessPaymentNotificationCommandHandler> logger)
    {
        _paymentGateway = paymentGateway;
        _paymentRepository = paymentRepository;
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ProcessPaymentNotificationResponse> Handle(
        ProcessPaymentNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var notification = await _paymentGateway.VerifyNotificationAsync(
            new PaymentNotificationRequest(
                request.RawBody,
                request.Signature),
            cancellationToken);

        if (!notification.IsVerified ||
            string.IsNullOrWhiteSpace(notification.PaymentReference) ||
            notification.AmountInMinorUnits is null ||
            string.IsNullOrWhiteSpace(notification.Currency) ||
            string.IsNullOrWhiteSpace(notification.ProviderTransactionId))
        {
            return new ProcessPaymentNotificationResponse(false);
        }

        var payment =
            await _paymentRepository.GetByExternalReferenceAsync(
                notification.PaymentReference,
                cancellationToken);

        if (payment is null)
        {
            _logger.LogWarning(
                "Received a verified Paymob notification for an unknown payment reference.");

            return new ProcessPaymentNotificationResponse(true);
        }

        var expectedAmountInMinorUnits =
            decimal.ToInt64(payment.Amount * 100m);

        if (notification.AmountInMinorUnits != expectedAmountInMinorUnits ||
            !string.Equals(
                notification.Currency,
                payment.Currency,
                StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogError(
                "Verified Paymob notification did not match payment {PaymentId}.",
                payment.Id);

            return new ProcessPaymentNotificationResponse(true);
        }

        if (notification.Status is PaymentNotificationStatus.Pending
            or PaymentNotificationStatus.Unrecognized)
        {
            return new ProcessPaymentNotificationResponse(true);
        }

        if (notification.Status == PaymentNotificationStatus.Failed)
        {
            if (payment.Status != PaymentStatus.Pending)
                return new ProcessPaymentNotificationResponse(true);

            await SaveInTransactionAsync(
                () =>
                {
                    payment.MarkAsFailed();
                    return Task.CompletedTask;
                },
                cancellationToken);

            return new ProcessPaymentNotificationResponse(true);
        }

        if (notification.Status != PaymentNotificationStatus.Succeeded)
            return new ProcessPaymentNotificationResponse(true);

        if (payment.Status == PaymentStatus.Refunded)
            return new ProcessPaymentNotificationResponse(true);

        if (payment.Status == PaymentStatus.Paid &&
            payment.ProviderTransactionId !=
            notification.ProviderTransactionId)
        {
            _logger.LogError(
                "A different successful Paymob transaction was received for already paid payment {PaymentId}.",
                payment.Id);

            return new ProcessPaymentNotificationResponse(true);
        }

        if (payment.EnrollmentProvisioningStatus ==
            EnrollmentProvisioningStatus.Completed)
        {
            return new ProcessPaymentNotificationResponse(true);
        }

        await SaveInTransactionAsync(
            async () =>
            {
                payment.MarkAsPaid(
                    notification.ProviderTransactionId);

                var course = await _courseRepository.GetByIdAsync(
                    payment.CourseId,
                    cancellationToken);

                if (course is null ||
                    course.Status == CourseStatus.Draft)
                {
                    payment.MarkEnrollmentProvisioningNeedsReview();
                    return;
                }

                var enrollment =
                    await _enrollmentRepository.GetByStudentAndCourseAsync(
                        payment.StudentId,
                        payment.CourseId,
                        cancellationToken);

                if (enrollment is null)
                {
                    enrollment = Enrollment.Create(
                        payment.StudentId,
                        payment.CourseId);

                    await _enrollmentRepository.AddAsync(
                        enrollment,
                        cancellationToken);

                    payment.MarkEnrollmentProvisioningCompleted();
                    return;
                }

                if (enrollment.Status == EnrollmentStatus.Cancelled)
                {
                    enrollment.Reactivate();
                    payment.MarkEnrollmentProvisioningCompleted();
                    return;
                }

                payment.MarkEnrollmentProvisioningNeedsReview();
            },
            cancellationToken);

        return new ProcessPaymentNotificationResponse(true);
    }

    private async Task SaveInTransactionAsync(
        Func<Task> update,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await update();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(
                CancellationToken.None);

            throw;
        }
    }
}