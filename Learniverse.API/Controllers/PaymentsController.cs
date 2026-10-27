using Learniverse.API.Contracts.Requests.Payments;
using Learniverse.Application.Features.Payments.Commands.StartPayment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learniverse.API.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize(Roles = "Student")]
public sealed class PaymentsController : ControllerBase
{
    private readonly ISender _sender;

    public PaymentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartPayment(
        StartPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new StartPaymentCommand(
                request.CourseId,
                request.PhoneNumber),
            cancellationToken);

        return Ok(result);
    }
}