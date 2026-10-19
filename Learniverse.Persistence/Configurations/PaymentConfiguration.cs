using Learniverse.Domain.Entities;
using Learniverse.Domain.Enums;
using Learniverse.Domain.Enum;
using Learniverse.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learniverse.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.StudentId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(p => p.CourseId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(p => p.EnrollmentProvisioningStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();
        builder.Property(p => p.ExternalReference)
            .HasMaxLength(256);

        builder.Property(p => p.ProviderTransactionId)
            .HasMaxLength(256);

        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();

        builder.Property(p => p.ExpiresAtUtc)
            .IsRequired();

        builder.HasIndex(p => new { p.StudentId, p.CourseId });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(p => p.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(p => p.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}