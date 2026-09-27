using MediatR;

namespace Learniverse.Application.Features.Sections.Queries.GetAllSections;

public sealed record GetAllSectionsQuery
    : IRequest<IReadOnlyList<GetAllSectionsResponse>>;