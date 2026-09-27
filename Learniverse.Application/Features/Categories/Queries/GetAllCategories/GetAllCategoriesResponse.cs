using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Categories.Queries.GetAllCategories;

public sealed record GetAllCategoriesResponse(
      Guid Id,
      string Name,
      string? CreatedBy,
      DateTime CreatedAtUtc
    );
