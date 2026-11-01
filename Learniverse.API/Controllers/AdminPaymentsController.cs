using Learniverse.Application.Features.Payments.Commands.RefundPayment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learniverse.API.Controllers;

[ApiController]
[Route("api/admin/payments")]
[Authorize(Roles = "Admin")]
public sealed class AdminPaymentsController : ControllerBase
{
    private readonly ISender _sender;

    public AdminPaymentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("{paymentId:guid}/refund")]
    [ProducesResponseType(typeof(RefundPaymentResponse), StatusCodes.Status202Accepted)]
    public async Task<ActionResult<RefundPaymentResponse>> Refund(
        Guid paymentId,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(
            new RefundPaymentCommand(paymentId),
            cancellationToken);

        return Accepted(response);
    }

    [HttpPost("{paymentId:guid}/refund/reconcile")]
    [ProducesResponseType(typeof(ReconcileRefundResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReconcileRefundResponse>> ReconcileRefund(
        Guid paymentId,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(
            new ReconcileRefundCommand(paymentId),
            cancellationToken);

        return Ok(response);
    }
}
