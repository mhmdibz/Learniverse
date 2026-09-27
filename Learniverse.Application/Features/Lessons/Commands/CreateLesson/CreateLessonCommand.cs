using System;
using System.Collections.Generic;
using System.Text;

using Learniverse.Domain.Enums;
using MediatR;

namespace Learniverse.Application.Features.Lessons.Commands.CreateLesson;

public sealed record CreateLessonCommand(
    Guid SectionId,
    string Title,
    string Description,
    int Order,
    ContentType ContentType,
    bool IsPreview
) : IRequest<Guid>;