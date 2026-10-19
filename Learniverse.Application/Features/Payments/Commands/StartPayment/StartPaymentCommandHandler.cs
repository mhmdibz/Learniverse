using Learniverse.Application.Exceptions;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Payments;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using MediatR;

namespace Learniverse.Application.Features.Payments.Commands.StartPayment;

public sealed class StartPaymentCommandHandler
    : IRequestHandler<StartPaymentCommand, StartPaymentResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public StartPaymentCommandHandler(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<StartPaymentResponse> Handle(
        StartPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetPublishedByIdAsync(
            request.CourseId,
            cancellationToken);

        if (course is null)
            throw new NotFoundException(nameof(Course), request.CourseId);

        if (course.Price <= 0)
            throw new ConflictException(
                "This course does not require payment.");

        var studentId = _currentUserService.UserId;

        if (await _enrollmentRepository.HasAccessAsync(
            studentId,
            course.Id,
            cancellationToken))
        {
            throw new ConflictException(
                "You are already enrolled in this course.");
        }

        var now = DateTime.UtcNow;

        var activePayment = await _paymentRepository.GetActivePendingAsync(
            studentId,
            course.Id,
            now,
            cancellationToken);

        if (activePayment is not null)
        {
            throw new ConflictException(
                "An active payment attempt already exists for this course.");
        }

        var expiresAtUtc = now.AddMinutes(30);

        var payment = Payment.Create(
            studentId,
            course.Id,
            course.Price,
            "EGP",
            expiresAtUtc);

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var gatewayResult = await _paymentGateway.StartPaymentAsync(
            new StartPaymentRequest(
                payment.Id,
                decimal.ToInt64(payment.Amount * 100m),
                payment.Currency,
                course.Title,
                new PaymentCustomerDetails(request.PhoneNumber.Trim())),
            cancellationToken);

        if (gatewayResult.Status == PaymentSessionStatus.Rejected)
        {
            payment.MarkAsFailed();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            throw new PaymentProviderRejectedException();
        }

        if (gatewayResult.Status == PaymentSessionStatus.Unknown)
        {
            throw new PaymentOutcomeUnknownException();
        }

        if (string.IsNullOrWhiteSpace(gatewayResult.CheckoutUrl) ||
            string.IsNullOrWhiteSpace(gatewayResult.ExternalReference))
        {
            throw new PaymentOutcomeUnknownException();
        }

        payment.AttachExternalReference(gatewayResult.ExternalReference);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new StartPaymentResponse(
            payment.Id,
            gatewayResult.CheckoutUrl);
    }
}
