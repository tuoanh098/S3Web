namespace Student.Infrastructure.Persistence;
using Student.Domain.Entities;
using Microsoft.EntityFrameworkCore;


public class StudentDbContext : DbContext
{
    public StudentDbContext(DbContextOptions<StudentDbContext> opts) : base(opts) { }

    public DbSet<Student> Students => Set<Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.FullName).IsRequired().HasMaxLength(150);
            b.Property(x => x.Email).IsRequired().HasMaxLength(200);
            b.Property(x => x.JoinedAt).IsRequired();
            b.HasIndex(x => x.Email).IsUnique();
        });
    }
}
