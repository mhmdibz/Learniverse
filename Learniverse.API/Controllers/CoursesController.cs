using FluentValidation;
using Learniverse.API.Contracts.Responses;
using Learniverse.Application.Features.Courses.Commands.CreateCourse;
using Learniverse.Application.Features.Courses.Commands.DeleteCourse;
using Learniverse.Application.Features.Courses.Commands.UpdateCourse;
using Learniverse.Application.Features.Courses.Queries.GetAllCourses;
using Learniverse.Application.Features.Courses.Queries.GetCourseById;
using Learniverse.Application.Features.Sections.Command.CreateSection;
using Learniverse.Domain.Entities;
using Learniverse.Application.Features.Sections.Queries.GetSectionsByCourseId;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Learniverse.API.Contracts.Requests;
using Learniverse.Application.Features.Sections.Command.UpdateSection;
using Learniverse.Application.Features.Sections.Command.DeleteSection;
using Learniverse.Application.Features.Sections.Queries.GetSectionById;
using Microsoft.AspNetCore.Authorization;
namespace Learniverse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class CoursesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetCourseByIdQuery(id);
            var response = await _mediator.Send(query, cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor,Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(CreateCourseCommand command, CancellationToken cancellationToken)
        {
            var courseId = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = courseId },
                null);
        }
        [HttpGet]
        [ProducesResponseType(
    typeof(IReadOnlyList<GetAllCoursesResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllCoursesQuery();

            var response = await _mediator.Send(query, cancellationToken);
            return Ok(response);

        }
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Instructor,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, UpdateCourseCommand command, CancellationToken cancellationToken)
        {
            if (id != command.Id)
                return BadRequest("Route id does not match comand id. ");
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteCourseCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpPost("{courseId:guid}/sections")]
        [Authorize(Roles = "Instructor,Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateSection(
      Guid courseId,
      CreateSectionRequest request,
      CancellationToken cancellationToken)
        {
            var command = new CreateSectionCommand(
                courseId,
                request.Title,
                request.Description,
                request.Order);

            var sectionId = await _mediator.Send(
                command,
                cancellationToken);

            return StatusCode(
                StatusCodes.Status201Created,
                sectionId);
        }

        [HttpPut("{courseId:guid}/sections/{sectionId:guid}")]
        [Authorize(Roles = "Instructor,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSection(Guid courseId, Guid sectionId, UpdateSectionRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateSectionCommand(
                   sectionId,
                   courseId,
                   request.Title,
                   request.Description,
                   request.Order);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{courseId:guid}/sections/{sectionId:guid}")]
        [Authorize(Roles = "Instructor,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSection(Guid sectionId, Guid courseId, CancellationToken cancellationToken)
        {
            var command = new DeleteSectionCommand(sectionId, courseId);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpGet("{courseId:guid}/sections")]
        [ProducesResponseType(typeof(IReadOnlyList<GetSectionsByCourseIdResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSectionsByCourseId(Guid courseId, CancellationToken cancellationToken)
        {
            var query = new GetSectionsByCourseIdQuery(courseId);
            var response = await _mediator.Send(query, cancellationToken);
            return Ok(response);
        }
    }
}
