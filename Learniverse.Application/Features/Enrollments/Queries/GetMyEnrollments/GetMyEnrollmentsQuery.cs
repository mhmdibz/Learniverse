using System.Collections.Generic;
using MediatR;

namespace Learniverse.Application.Features.Enrollments.Queries.GetMyEnrollments;

public sealed record GetMyEnrollmentsQuery
    : IRequest<IReadOnlyList<GetMyEnrollmentsResponse>>;
