using Learniverse.Application.Exceptions;
using Learniverse.Application.Features.Sections.Queries.GetSectionById;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enums;
using MediatR;

namespace Learniverse.Application.Features.Sections.Queries.GetSectionById;

public sealed class GetSectionByIdQueryHandler
    : IRequestHandler<GetSectionByIdQuery, GetSectionByIdResponse>
{
    private readonly ISectionRepository _sectionRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseVisibilityPolicy _courseVisibilityPolicy;

    public GetSectionByIdQueryHandler(
        ISectionRepository sectionRepository,
        ICourseRepository courseRepository,
        ICourseVisibilityPolicy courseVisibilityPolicy)
    {
        _sectionRepository = sectionRepository;
        _courseRepository = courseRepository;
        _courseVisibilityPolicy = courseVisibilityPolicy;
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

        if (course is null)
        {
            throw new NotFoundException(nameof(Course), section.CourseId);
        }

        if (course.Status != CourseStatus.Published &&
            !_courseVisibilityPolicy.IsAdminOrOwner(course.InstructorId))
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