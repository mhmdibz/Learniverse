using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace Learniverse.Application.Features.Courses.Commands.CreateCourse;

public record CreateCourseCommand(
    string Title,
    string Description,
    decimal Price,
    Guid CategoryId
) : IRequest<Guid>;