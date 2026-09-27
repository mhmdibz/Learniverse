using Learniverse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learniverse.Persistence.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
               .ValueGeneratedNever();
        builder.Property(l => l.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(l => l.Description)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(l => l.Order)
               .IsRequired();

        builder.Property(l => l.ContentType)
               .HasConversion<string>()
               .HasMaxLength(20);

        builder.Property(l => l.IsPreview)
               .IsRequired();

        builder.Property(l => l.SectionId)
               .IsRequired();

        builder.HasIndex(l => new { l.SectionId, l.Order })
         .IsUnique()
         .HasFilter("[IsDeleted] = 0");
    }
}

