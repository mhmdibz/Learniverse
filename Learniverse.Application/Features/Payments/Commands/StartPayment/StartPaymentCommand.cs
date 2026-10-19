using MediatR;

namespace Learniverse.Application.Features.Payments.Commands.StartPayment;

public sealed record StartPaymentCommand(
    Guid CourseId,
    string PhoneNumber) : IRequest<StartPaymentResponse>;

public sealed record StartPaymentResponse(
    Guid PaymentId,
    string CheckoutUrl);