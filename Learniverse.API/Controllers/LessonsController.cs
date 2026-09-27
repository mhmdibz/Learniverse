using Learniverse.Application.Features.Lessons.Queries.GetAllLessons;
using Learniverse.Application.Features.Lessons.Queries.GetLessonById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Learniverse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LessonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{lessonId:guid}")]
    [ProducesResponseType(typeof(GetLessonByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLessonById(
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        var query = new GetLessonByIdQuery(lessonId);

        var response = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<GetAllLessonsResponse>),
        StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllLessons(
        CancellationToken cancellationToken)
    {
        var query = new GetAllLessonsQuery();

        var response = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(response);
    }


}