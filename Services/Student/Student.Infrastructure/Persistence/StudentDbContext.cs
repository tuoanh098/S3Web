using Microsoft.EntityFrameworkCore;

public class StudentDbContext : DbContext
{
    public StudentDbContext(DbContextOptions<StudentDbContext> opts) : base(opts) { }

    public DbSet<Student.Domain.Entities.Student> Students => Set<Student.Domain.Entities.Student>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
