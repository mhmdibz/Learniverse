using Learniverse.Application.Common.Constants;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;

namespace Learniverse.Application.Features.Categories.Queries.GetAllCategories;

public sealed class GetAllCategoriesQueryHandler
    : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<GetAllCategoriesResponse>>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

    private readonly ICategoryRepository _categoryRepository;
    private readonly ICacheService _cacheService;

    public GetAllCategoriesQueryHandler(
        ICategoryRepository categoryRepository,
        ICacheService cacheService)
    {
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
    }

    public async Task<IReadOnlyList<GetAllCategoriesResponse>> Handle(
        GetAllCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        return await _cacheService.GetOrCreateAsync<
            IReadOnlyList<GetAllCategoriesResponse>>(
            CategoryCacheKeys.All,
            CategoryCacheKeys.All,
            async cacheCancellationToken =>
            {
                var categories = await _categoryRepository.GetAllAsync(
                    cacheCancellationToken);

                return categories
                    .Select(category => new GetAllCategoriesResponse(
                        category.Id,
                        category.Name,
                        category.CreatedBy,
                        category.CreatedAtUtc))
                    .ToList();
            },
            CacheExpiration,
            cancellationToken);
    }
}
