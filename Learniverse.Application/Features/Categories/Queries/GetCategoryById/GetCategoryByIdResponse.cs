using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdResponse(
    Guid Id,
    string Name,
    DateTime CreatedAtUtc
);