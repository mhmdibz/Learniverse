using MediatR;

namespace Learniverse.Application.Features.Lessons.Queries.GetAllLessons;

public sealed record GetAllLessonsQuery
    : IRequest<IReadOnlyList<GetAllLessonsResponse>>;