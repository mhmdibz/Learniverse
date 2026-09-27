using Learniverse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Learniverse.Persistence.Configurations;

public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => new { s.CourseId, s.Order })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0"); builder.Property(s => s.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(s => s.Description)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(s => s.Order)
               .IsRequired();

        builder.Property(s => s.CourseId)
               .IsRequired();

        builder.HasMany(s => s.Lessons)
               .WithOne()
               .HasForeignKey(l => l.SectionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(s => s.Lessons)
               .UsePropertyAccessMode(PropertyAccessMode.Field);
        builder.Property(s => s.Id)
       .ValueGeneratedNever();
    }



}