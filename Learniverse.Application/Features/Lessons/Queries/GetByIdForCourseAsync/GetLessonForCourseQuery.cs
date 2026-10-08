using Learniverse.Application.Features.Lessons.Queries.GetLessonById;
using MediatR;

namespace Learniverse.Application.Features.Lessons.Queries.GetLessonForCourse;

public sealed record GetLessonForCourseQuery(
    Guid CourseId,
    Guid LessonId)
    : IRequest<GetLessonForCourseResponse>;