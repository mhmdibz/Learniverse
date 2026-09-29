using Learniverse.API.Contracts.Lessons;
using Learniverse.API.Contracts.Responses;
using Learniverse.Application.Features.Lessons.Commands.CreateLesson;
using Learniverse.Application.Features.Lessons.Commands.DeleteLesson;
using Learniverse.Application.Features.Lessons.Commands.UpdateLesson;
using Learniverse.Application.Features.Sections.Queries.GetAllSections;
using Learniverse.Application.Features.Sections.Queries.GetSectionById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learniverse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SectionsController : ControllerBase
{
    private readonly IMediator _mediator;
    public SectionsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet("{sectionId:guid}")]
    [ProducesResponseType(typeof(GetSectionByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSectionById(Guid sectionId, CancellationToken cancellationToken)
    {
        var query = new GetSectionByIdQuery(sectionId);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }
    [HttpGet]
    [ProducesResponseType(
    typeof(IReadOnlyList<GetAllSectionsResponse>),
    StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllSections(
    CancellationToken cancellationToken)
    {
        var query = new GetAllSectionsQuery();

        var response = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("{sectionId:guid}/lessons")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateLesson(Guid sectionId, CreateLessonRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateLessonCommand(
                sectionId,
                request.Title,
                request.Description,
                request.Order,
                request.ContentType,
                request.IsPreview);

        var lessonId = await _mediator.Send(
       command,
       cancellationToken);

        return CreatedAtAction(
    actionName: "GetLessonById",
    controllerName: "Lessons",
    routeValues: new { lessonId },
    value: lessonId);
    }
    [HttpPut("{sectionId:guid}/lessons/{lessonId:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLesson(
     Guid sectionId,
     Guid lessonId,
     UpdateLessonRequest request,
     CancellationToken cancellationToken)
    {
        var command = new UpdateLessonCommand(
             sectionId,
             lessonId,
             request.Title,
             request.Description,
             request.Order,
             request.ContentType,
             request.IsPreview);

        await _mediator.Send(
            command,
            cancellationToken);

        return NoContent();
    }
    [HttpDelete("{sectionId:guid}/lessons/{lessonId:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLesson(
   Guid sectionId,
   Guid lessonId,
   CancellationToken cancellationToken)
    {
        var command = new DeleteLessonCommand(
            sectionId,
            lessonId);

        await _mediator.Send(
            command,
            cancellationToken);

        return NoContent();
    }

}