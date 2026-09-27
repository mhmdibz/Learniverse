using MediatR;

namespace Learniverse.Application.Features.Lessons.Queries.GetLessonById;

public sealed record GetLessonByIdQuery(
    Guid Id
) : IRequest<GetLessonByIdResponse>;