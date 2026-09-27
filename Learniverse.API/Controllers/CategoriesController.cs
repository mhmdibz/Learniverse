using Learniverse.API.Contracts.Responses;
using Learniverse.Application.Features.Categories.Commands.CreateCategory;
using Learniverse.Application.Features.Categories.Queries.GetAllCategories;
using Learniverse.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Learniverse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetCategoryByIdResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(id);
        var response = await _mediator.Send(query, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var categoryId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = categoryId },
            null);
    }

    [HttpGet]
    [ProducesResponseType(
    typeof(IReadOnlyList<GetAllCategoriesResponse>),
    StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
    CancellationToken cancellationToken)
    {
        var query = new GetAllCategoriesQuery();

        var response = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(response);
    }

}