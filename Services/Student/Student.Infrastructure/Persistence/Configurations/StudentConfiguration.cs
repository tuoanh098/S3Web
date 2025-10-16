using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StudentConfiguration : IEntityTypeConfiguration<Student.Domain.Entities.Student>
{
    public void Configure(EntityTypeBuilder<Student.Domain.Entities.Student> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.UserId).IsRequired().HasMaxLength(450);
        e.HasIndex(x => x.UserId).IsUnique();
        e.Property(x => x.FirstName).HasMaxLength(100);
        e.Property(x => x.LastName).HasMaxLength(100);
        e.Property(x => x.Nation).HasMaxLength(100);
        e.Property(x => x.Gender).HasMaxLength(50);
        e.Property(x => x.Mobile).HasMaxLength(50);
        e.Property(x => x.Parent).HasMaxLength(200);
        e.Property(x => x.Bio).HasColumnType("nvarchar(max)");
        e.Property(x => x.JoinedAt).IsRequired();
    }
}
