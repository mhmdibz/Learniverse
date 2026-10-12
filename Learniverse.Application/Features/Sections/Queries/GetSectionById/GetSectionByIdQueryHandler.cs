using Learniverse.Application.Exceptions;
using Learniverse.Application.Features.Sections.Queries.GetSectionById;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using MediatR;

namespace Learniverse.Application.Features.Sections.Queries.GetSectionById;

public sealed class GetSectionByIdQueryHandler
    : IRequestHandler<GetSectionByIdQuery, GetSectionByIdResponse>
{
    private readonly ISectionRepository _sectionRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseContentAccessService _courseContentAccessService;

    public GetSectionByIdQueryHandler(
        ISectionRepository sectionRepository,
        ICourseRepository courseRepository,
        ICourseContentAccessService courseContentAccessService)
    {
        _sectionRepository = sectionRepository;
        _courseRepository = courseRepository;
        _courseContentAccessService = courseContentAccessService;
    }

    public async Task<GetSectionByIdResponse> Handle(
        GetSectionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var section =
            await _sectionRepository.GetByIdIncludingUnpublishedAsync(
                request.Id,
                cancellationToken);

        if (section is null)
        {
            throw new NotFoundException(nameof(Section), request.Id);
        }

        var course = await _courseRepository.GetByIdAsync(
            section.CourseId,
            cancellationToken);

        if (course is null ||
            !await _courseContentAccessService.CanViewCourseAsync(
                course,
                cancellationToken))
        {
            throw new NotFoundException(nameof(Section), request.Id);
        }

        return new GetSectionByIdResponse(
            section.Id,
            section.Title,
            section.Description,
            section.Order,
            section.CourseId);
    }
}