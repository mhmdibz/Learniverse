using MediatR;

namespace Learniverse.Application.Features.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id)
    : IRequest<GetCategoryByIdResponse>;