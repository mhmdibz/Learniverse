using System;
using System.Collections.Generic;
using System.Text;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;

namespace Learniverse.Application.Features.Sections.Queries.GetAllSections;

public sealed class GetAllSectionsQueryHandler
    : IRequestHandler<GetAllSectionsQuery, IReadOnlyList<GetAllSectionsResponse>>
{
    private readonly ISectionRepository _sectionRepository;

    public GetAllSectionsQueryHandler(
        ISectionRepository sectionRepository)
    {
        _sectionRepository = sectionRepository;
    }

    public async Task<IReadOnlyList<GetAllSectionsResponse>> Handle(
        GetAllSectionsQuery request,
        CancellationToken cancellationToken)
    {
        var sections = await _sectionRepository.GetAllAsync(
            cancellationToken);

        return sections
            .Select(s => new GetAllSectionsResponse(
                s.Id,
                s.Title,
                s.Description,
                s.Order,
                s.CourseId))
            .ToList();
    }
}