using Learniverse.Application.Exceptions;
using Learniverse.Application.Features.Courses.Queries.GetCourseById;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using MediatR;

namespace Learniverse.Application.Features.Courses.Queries.GetCourseById;

public sealed class GetCourseByIdQueryHandler
    : IRequestHandler<GetCourseByIdQuery, GetCourseByIdResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly ICourseContentAccessService _courseContentAccessService;

    public GetCourseByIdQueryHandler(
        ICourseRepository courseRepository,
        ICourseContentAccessService courseContentAccessService)
    {
        _courseRepository = courseRepository;
        _courseContentAccessService = courseContentAccessService;
    }

    public async Task<GetCourseByIdResponse> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken)
    {
        var course = await _courseRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (course is null ||
            !await _courseContentAccessService.CanViewCourseAsync(
                course,
                cancellationToken))
        {
            throw new NotFoundException(nameof(Course), request.Id);
        }

        return new GetCourseByIdResponse(
            course.Id,
            course.Title,
            course.Description,
            course.Price,
            course.Status,
            course.InstructorId,
            course.CategoryId,
            course.Category.Name,
            course.CreatedAtUtc,
            course.UpdatedAtUtc);
    }
}