namespace Student.Domain.Entities;

public class Student
{
    public long Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
