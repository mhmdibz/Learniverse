using MediatR;

namespace Learniverse.Application.Features.Lessons.Queries.GetLessonsForCourse;

public sealed record GetLessonsForCourseQuery(Guid CourseId)
    : IRequest<IReadOnlyList<GetLessonsForCourseResponse>>;