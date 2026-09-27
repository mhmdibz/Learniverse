using Learniverse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learniverse.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(c => c.Description)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(c => c.Price)
          .HasColumnType("decimal(18,2)");

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.InstructorId)
               .IsRequired();

        builder.Property(c => c.CategoryId)
               .IsRequired();

        builder.HasMany(c => c.Sections)
               .WithOne()
               .HasForeignKey(s => s.CourseId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Sections)
       .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
