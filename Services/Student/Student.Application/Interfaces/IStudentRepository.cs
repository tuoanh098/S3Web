namespace Student.Application.Interfaces;
using Student.Domain.Entities;
public interface IStudentRepository
{
    Task<Student> AddAsync(Student s, CancellationToken ct = default);
    Task<Student?> GetByEmailAsync(string email, CancellationToken ct = default);

    Task<Student?> GetAsync(long id, CancellationToken ct = default);
    Task<List<Student>> ListAsync(CancellationToken ct = default);
    Task<bool> UpdateAsync(Student s, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
