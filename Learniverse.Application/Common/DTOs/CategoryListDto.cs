using System;
using System.Collections.Generic;
using System.Text;
namespace Learniverse.Application.Common.DTOs;

public sealed record CategoryListDto(
    Guid Id,
    string Name,
    string? CreatedBy,
    DateTime CreatedAtUtc
);