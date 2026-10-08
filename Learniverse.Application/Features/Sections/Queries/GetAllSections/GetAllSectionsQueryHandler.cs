using Learniverse.Application.Common.Constants;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;

namespace Learniverse.Application.Features.Sections.Queries.GetAllSections;

public sealed class GetAllSectionsQueryHandler
    : IRequestHandler<
        GetAllSectionsQuery,
        IReadOnlyList<GetAllSectionsResponse>>
{
    private readonly ISectionRepository _sectionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAllSectionsQueryHandler(
        ISectionRepository sectionRepository,
        ICurrentUserService currentUserService)
    {
        _sectionRepository = sectionRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<GetAllSectionsResponse>> Handle(
        GetAllSectionsQuery request,
        CancellationToken cancellationToken)
    {
        var roles = _currentUserService.Roles;

        var isAdmin = roles.Contains(Roles.Admin);

        var instructorId = roles.Contains(Roles.Instructor)
            ? _currentUserService.UserIdOrNull
            : null;

        var sections = await _sectionRepository.GetAllAsync(
            isAdmin,
            instructorId,
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