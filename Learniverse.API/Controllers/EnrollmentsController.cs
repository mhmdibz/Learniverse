using System.Collections.Generic;
using Learniverse.Application.Features.Enrollments.Queries.GetMyEnrollments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learniverse.API.Controllers;

[ApiController]
[Route("api/enrollments")]
[Authorize(Roles = "Student")]
public sealed class EnrollmentsController : ControllerBase
{
    private readonly ISender _sender;

    public EnrollmentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("me")]
    [ProducesResponseType(
        typeof(IReadOnlyList<GetMyEnrollmentsResponse>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyEnrollments(
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(
            new GetMyEnrollmentsQuery(),
            cancellationToken);

        return Ok(response);
    }
}
