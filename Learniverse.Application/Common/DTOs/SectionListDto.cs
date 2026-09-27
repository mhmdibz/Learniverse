using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Common.DTOs;

public sealed record SectionListDto(
    Guid Id,
    string Title,
    string Description,
    int Order,
    Guid CourseId);