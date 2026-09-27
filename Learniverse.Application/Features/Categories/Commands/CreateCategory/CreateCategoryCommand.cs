using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Learniverse.Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name
) : IRequest<Guid>;