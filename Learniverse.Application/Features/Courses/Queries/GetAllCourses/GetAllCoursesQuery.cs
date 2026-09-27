using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Learniverse.Application.Features.Courses.Queries.GetAllCourses;

public sealed record GetAllCoursesQuery
    : IRequest<IReadOnlyList<GetAllCoursesResponse>>;