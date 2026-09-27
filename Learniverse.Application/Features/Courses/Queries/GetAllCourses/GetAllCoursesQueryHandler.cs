using Learniverse.Application.Interfaces.Repositories;
using MediatR;

namespace Learniverse.Application.Features.Courses.Queries.GetAllCourses;

public sealed class GetAllCoursesQueryHandler
    : IRequestHandler<
        GetAllCoursesQuery,
        IReadOnlyList<GetAllCoursesResponse>>
{
    private readonly ICourseRepository _courseRepository;

    public GetAllCoursesQueryHandler(
        ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<IReadOnlyList<GetAllCoursesResponse>> Handle(
    GetAllCoursesQuery request,
    CancellationToken cancellationToken)
    {
        var courses = await _courseRepository.GetAllAsync(
            cancellationToken);

        return courses
            .Select(course => new GetAllCoursesResponse(
                course.Id,
                course.Title,
                course.Price,
                course.Status,
                course.InstructorId,
                course.CategoryId,
                course.CategoryName,
                course.CreatedAtUtc))
            .ToList();
    }
}