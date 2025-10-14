using Microsoft.EntityFrameworkCore;
using Students.Api.Domain;

namespace Students.Api.Infrastructure;

public sealed class StudentsDb : DbContext
{
    public StudentsDb(DbContextOptions<StudentsDb> options) : base(options) { }
    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Student>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
        });
    }
}
