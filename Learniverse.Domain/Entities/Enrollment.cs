using Learniverse.Domain.Common;
using Learniverse.Domain.Enums;
using Learniverse.Domain.Exceptions;

namespace Learniverse.Domain.Entities;

public class Enrollment : BaseEntity
{
    private Enrollment() { }

    private Enrollment(string studentId, Guid courseId)
    {
        Id = Guid.CreateVersion7();
        StudentId = studentId;
        CourseId = courseId;
        Status = EnrollmentStatus.Active;
        EnrolledAtUtc = DateTime.UtcNow;
    }

    public string StudentId { get; private set; } = default!;
    public Guid CourseId { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public DateTime EnrolledAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }

    public static Enrollment Create(string studentId, Guid courseId)
    {
        if (string.IsNullOrWhiteSpace(studentId))
            throw new DomainException("Student ID is required.");

        if (courseId == Guid.Empty)
            throw new DomainException("Course ID is required.");

        return new Enrollment(studentId, courseId);
    }

    public void Complete()
    {
        if (Status != EnrollmentStatus.Active)
            throw new DomainException("Only active enrollments can be completed.");

        Status = EnrollmentStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == EnrollmentStatus.Cancelled)
            return;

        Status = EnrollmentStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
    }
}