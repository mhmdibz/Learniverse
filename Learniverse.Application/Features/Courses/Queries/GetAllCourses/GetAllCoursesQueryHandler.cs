using Learniverse.Application.Common.Constants;
using Learniverse.Application.Common.Models;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;

namespace Learniverse.Application.Features.Courses.Queries.GetAllCourses;

public sealed class GetAllCoursesQueryHandler
    : IRequestHandler<
        GetAllCoursesQuery,
        PagedResult<GetAllCoursesResponse>>
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

    private readonly ICourseRepository _courseRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICacheService _cacheService;

    public GetAllCoursesQueryHandler(
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService,
        ICacheService cacheService)
    {
        _courseRepository = courseRepository;
        _currentUserService = currentUserService;
        _cacheService = cacheService;
    }

    public async Task<PagedResult<GetAllCoursesResponse>> Handle(
        GetAllCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var roles = _currentUserService.Roles;

        var isAdmin = roles.Contains(Roles.Admin);
        var instructorId = _currentUserService.UserIdOrNull;

        if (!_currentUserService.IsAuthenticated)
        {
            return await _cacheService.GetOrCreateAsync(
                CourseCacheKeys.PublicList(
                    request.PageNumber,
                    request.PageSize),
                CourseCacheKeys.PublicListTag,
                cacheCancellationToken => GetPageAsync(
                    isAdmin: false,
                    instructorId: null,
                    request,
                    cacheCancellationToken),
                CacheExpiration,
                cancellationToken);
        }

        return await GetPageAsync(
            isAdmin,
            instructorId,
            request,
            cancellationToken);
    }

    private async Task<PagedResult<GetAllCoursesResponse>> GetPageAsync(
        bool isAdmin,
        string? instructorId,
        GetAllCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var page = await _courseRepository.GetAllAsync(
            isAdmin,
            instructorId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var items = page.Items
            .Select(course => new GetAllCoursesResponse(
                course.Id,
                course.Title,
                course.Price,
                course.Status,
                course.InstructorId,
                course.CategoryId,
                course.CategoryName,
                course.CreatedAtUtc))
            .ToList();

        return new PagedResult<GetAllCoursesResponse>(
            items,
            page.PageNumber,
            page.PageSize,
            page.TotalCount);
    }
}
