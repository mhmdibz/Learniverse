using Learniverse.Application.Interfaces.Repositories;
using Learniverse.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Learniverse.Application.Features.Categories.Queries.GetAllCategories
{
    public sealed class GetAllCategoriesQueryHandler
        : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<GetAllCategoriesResponse>>
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IReadOnlyList<GetAllCategoriesResponse>> Handle(
     GetAllCategoriesQuery request,
     CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync(
                cancellationToken);

            return categories
                .Select(category => new GetAllCategoriesResponse(
                    category.Id,
                    category.Name,
                    category.CreatedBy,
                    category.CreatedAtUtc))
                .ToList();
        }
    }
}
