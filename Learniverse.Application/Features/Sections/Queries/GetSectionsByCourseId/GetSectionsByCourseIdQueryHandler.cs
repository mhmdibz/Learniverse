using Learniverse.Application.Exceptions;
using Learniverse.Application.Features.Sections.Queries.GetSectionsByCourseId;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using Learniverse.Domain.Enums;
using MediatR;

namespace Learniverse.Application.Features.Sections.Queries.GetSectionsByCourseId;

public sealed class GetSectionsByCourseIdQueryHandler
    : IRequestHandler<
        GetSectionsByCourseIdQuery,
        IReadOnlyList<GetSectionsByCourseIdResponse>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ICourseVisibilityPolicy _courseVisibilityPolicy;

    public GetSectionsByCourseIdQueryHandler(
        ICourseRepository courseRepository,
        ISectionRepository sectionRepository,
        ICourseVisibilityPolicy courseVisibilityPolicy)
    {
        _courseRepository = courseRepository;
        _sectionRepository = sectionRepository;
        _courseVisibilityPolicy = courseVisibilityPolicy;
    }

    public async Task<IReadOnlyList<GetSectionsByCourseIdResponse>> Handle(
        GetSectionsByCourseIdQuery request,
        CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(
            request.CourseId,
            cancellationToken);

        if (course is null)
        {
            throw new NotFoundException(
                nameof(Course),
                request.CourseId);
        }

        if (course.Status != CourseStatus.Published &&
            !_courseVisibilityPolicy.IsAdminOrOwner(course.InstructorId))
        {
            throw new NotFoundException(
                nameof(Course),
                request.CourseId);
        }

        var sections =
            await _sectionRepository.GetByCourseIdIncludingUnpublishedAsync(
                request.CourseId,
                cancellationToken);

        return sections
            .Select(section => new GetSectionsByCourseIdResponse(
                section.Id,
                section.Title,
                section.Description,
                section.Order,
                section.CourseId))
            .ToList();
    }
}