using MediatR;

namespace Learniverse.Application.Features.Payments.Commands.ProcessPaymentNotification;

public sealed record ProcessPaymentNotificationCommand(
    string RawBody,
    string Signature)
    : IRequest<ProcessPaymentNotificationResponse>;

public sealed record ProcessPaymentNotificationResponse(
    bool Acknowledged);