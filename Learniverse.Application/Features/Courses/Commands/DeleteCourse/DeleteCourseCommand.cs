using System;
using System.Collections.Generic;
using System.Text;

using MediatR;

namespace Learniverse.Application.Features.Courses.Commands.DeleteCourse;

public sealed record DeleteCourseCommand(Guid Id)
    : IRequest;