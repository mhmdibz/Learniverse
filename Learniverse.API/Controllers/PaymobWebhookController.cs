using Learniverse.Application.Features.Payments.Commands.ProcessPaymentNotification;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learniverse.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/payments/paymob")]
public sealed class PaymobWebhookController : ControllerBase
{
    private readonly ISender _sender;

    public PaymobWebhookController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("notification")]
    public async Task<IActionResult> ReceiveNotification(
        CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Request.Body);
        var rawBody = await reader.ReadToEndAsync(cancellationToken);
        var signature = Request.Query["hmac"].ToString();

        var result = await _sender.Send(
            new ProcessPaymentNotificationCommand(rawBody, signature),
            cancellationToken);

        if (!result.Acknowledged)
            return Unauthorized();

        return Ok();
    }
}