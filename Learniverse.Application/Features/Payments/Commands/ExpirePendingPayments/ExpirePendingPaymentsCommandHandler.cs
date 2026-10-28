using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;

namespace Learniverse.Application.Features.Payments.Commands.ExpirePendingPayments;

public sealed class ExpirePendingPaymentsCommandHandler
    : IRequestHandler<ExpirePendingPaymentsCommand, int>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ExpirePendingPaymentsCommandHandler(
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(
        ExpirePendingPaymentsCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var expiredCount = await _paymentRepository.ExpirePendingAsync(
                DateTime.UtcNow,
                cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return expiredCount;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(
                CancellationToken.None);

            throw;
        }
    }
}
