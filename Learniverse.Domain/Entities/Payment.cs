using Learniverse.Domain.Common;
using Learniverse.Domain.Enum;
using Learniverse.Domain.Enums;
using Learniverse.Domain.Exceptions;

namespace Learniverse.Domain.Entities;

public class Payment : BaseEntity
{
    private Payment() { }

    private Payment(
      string studentId,
      Guid courseId,
      decimal amount,
      DateTime expiresAtUtc)
    {
        Id = Guid.CreateVersion7();
        StudentId = studentId;
        CourseId = courseId;
        Amount = amount;
        Currency = "EGP";
        Status = PaymentStatus.Pending;
        CreatedAtUtc = DateTime.UtcNow;
        ExpiresAtUtc = expiresAtUtc;
    }

    public string StudentId { get; private set; } = default!;
    public Guid CourseId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "EGP";
    public PaymentStatus Status { get; private set; }
    public EnrollmentProvisioningStatus EnrollmentProvisioningStatus
    {
        get;
        private set;
    } = EnrollmentProvisioningStatus.NotStarted;

    // Reference of the payment session at the provider (set after the gateway accepts the request).
    public string? ExternalReference { get; private set; }

    // Provider transaction id of the confirmed payment (set from the verified notification).
    public string? ProviderTransactionId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }
    public DateTime? RefundedAtUtc { get; private set; }

    public static Payment Create(
        string studentId,
        Guid courseId,
        decimal amount,
        string currency,
        DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(studentId))
            throw new DomainException("Student ID is required.");

        if (courseId == Guid.Empty)
            throw new DomainException("Course ID is required.");

        if (amount <= 0)
            throw new DomainException("Payment amount must be greater than zero.");

        if (!string.Equals(
         currency?.Trim(),
         "EGP",
         StringComparison.OrdinalIgnoreCase))
            throw new DomainException("Only EGP payments are supported.");


        if (expiresAtUtc <= DateTime.UtcNow)
            throw new DomainException("Payment expiry must be in the future.");

        return new Payment(
           studentId.Trim(),
           courseId,
           amount,
           expiresAtUtc);
    }

    public void AttachExternalReference(string externalReference)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("External reference can only be set on a pending payment.");

        if (string.IsNullOrWhiteSpace(externalReference))
            throw new DomainException("External reference is required.");

        externalReference = externalReference.Trim();

        if (ExternalReference is not null)
        {
            if (ExternalReference == externalReference)
                return;

            throw new DomainException(
                "External reference cannot be changed once attached.");
        }

        ExternalReference = externalReference;
    }

    public void MarkAsPaid(string providerTransactionId)
    {
        if (string.IsNullOrWhiteSpace(providerTransactionId))
            throw new DomainException("Provider transaction id is required.");

        // Same confirmation delivered again: nothing to change.
        if (Status == PaymentStatus.Paid &&
            ProviderTransactionId == providerTransactionId)
            return;

        //Expired is allowed: a verified success can arrive after our timeout.
        if (Status != PaymentStatus.Pending &&
            Status != PaymentStatus.Expired &&
            Status != PaymentStatus.Failed)
            throw new DomainException($"Cannot mark a {Status} payment as paid.");

        Status = PaymentStatus.Paid;
        ProviderTransactionId = providerTransactionId;
        PaidAtUtc = DateTime.UtcNow;
    }

    public void MarkEnrollmentProvisioningCompleted()
    {
        if (Status != PaymentStatus.Paid)
            throw new DomainException(
                "Enrollment provisioning can only complete for a paid payment.");

        if (EnrollmentProvisioningStatus ==
            EnrollmentProvisioningStatus.Completed)
            return;

        EnrollmentProvisioningStatus =
            EnrollmentProvisioningStatus.Completed;
    }

    public void MarkEnrollmentProvisioningNeedsReview()
    {
        if (Status != PaymentStatus.Paid)
            throw new DomainException(
                "Enrollment provisioning review requires a paid payment.");

        if (EnrollmentProvisioningStatus ==
            EnrollmentProvisioningStatus.Completed)
            throw new DomainException(
                "Completed enrollment provisioning cannot need review.");

        EnrollmentProvisioningStatus =
            EnrollmentProvisioningStatus.NeedsReview;
    }

    public void MarkAsFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException(
                $"Cannot mark a {Status} payment as failed.");

        Status = PaymentStatus.Failed;
    }

    public void MarkAsExpired()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException(
                $"Cannot mark a {Status} payment as expired.");

        if (DateTime.UtcNow < ExpiresAtUtc)
            throw new DomainException(
                "A payment cannot expire before its expiry time.");

        Status = PaymentStatus.Expired;
    }

    public void MarkAsRefunded()
    {
        if (Status == PaymentStatus.Refunded)
            return;

        if (Status != PaymentStatus.Paid)
            throw new DomainException(
                "Only a paid payment can be refunded.");

        Status = PaymentStatus.Refunded;
        RefundedAtUtc = DateTime.UtcNow;
    }
}
