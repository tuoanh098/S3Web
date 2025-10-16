namespace Student.Domain.Entities;

public sealed class Student
{
    public Guid Id { get; set; }                 // PK for student record
    public string UserId { get; set; } = null!; // FK to IdentityUser.Id (string)
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? Birthday { get; set; }
    public string? Gender { get; set; }
    public string? Nation { get; set; }
    public string? Mobile { get; set; }
    public string? Parent { get; set; }
    public string? Bio { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
