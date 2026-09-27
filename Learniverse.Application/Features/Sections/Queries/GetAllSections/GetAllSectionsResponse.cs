namespace Learniverse.Application.Features.Sections.Queries.GetAllSections;

public sealed record GetAllSectionsResponse(
    Guid Id,
    string Title,
    string Description,
    int Order,
    Guid CourseId);