using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Categories.Queries.GetAllCategories
{
    public sealed record GetAllCategoriesQuery
        : IRequest<IReadOnlyList<GetAllCategoriesResponse>>
    {

    }
}
