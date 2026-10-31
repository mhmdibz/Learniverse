using MediatR;

namespace Learniverse.Application.Features.Payments.Commands.ExpirePendingPayments;

public sealed record ExpirePendingPaymentsCommand
    : IRequest<int>;
