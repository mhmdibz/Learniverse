using System.Collections.Generic;
using System.Linq;
using Learniverse.Application.Interfaces.Common;
using Learniverse.Application.Interfaces.Repositories;
using MediatR;

namespace Learniverse.Application.Features.Enrollments.Queries.GetMyEnrollments;

public sealed class GetMyEnrollmentsQueryHandler
    : IRequestHandler<
        GetMyEnrollmentsQuery,
        IReadOnlyList<GetMyEnrollmentsResponse>>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyEnrollmentsQueryHandler(
        IEnrollmentRepository enrollmentRepository,
        ICurrentUserService currentUserService)
    {
        _enrollmentRepository = enrollmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<GetMyEnrollmentsResponse>> Handle(
        GetMyEnrollmentsQuery request,
        CancellationToken cancellationToken)
    {
        var studentId = _currentUserService.UserId;

        var enrollments =
            await _enrollmentRepository.GetStudentEnrollmentsAsync(
                studentId,
                cancellationToken);

        return enrollments
            .Select(enrollment => new GetMyEnrollmentsResponse(
                enrollment.CourseId,
                enrollment.CourseTitle,
                enrollment.Price,
                enrollment.CourseStatus,
                enrollment.EnrollmentStatus,
                enrollment.EnrolledAtUtc,
                enrollment.CompletedAtUtc))
            .ToList();
    }
}