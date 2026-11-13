using Learniverse.Application.Common.Models;
using MediatR;

namespace Learniverse.Application.Features.Courses.Queries.GetAllCourses;

public sealed record GetAllCoursesQuery(
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<PagedResult<GetAllCoursesResponse>>;
