namespace Students.Api.Domain;
public sealed class Student
{
    public long Id { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(long id);
    Task<List<Student>> GetAllAsync();
    Task AddAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(long id);
}   