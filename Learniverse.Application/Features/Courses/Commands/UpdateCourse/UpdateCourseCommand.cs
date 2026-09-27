using Learniverse.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Courses.Commands.UpdateCourse;

public sealed record UpdateCourseCommand(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    CourseStatus Status
) : IRequest;