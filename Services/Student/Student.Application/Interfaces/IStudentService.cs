namespace Student.Application.Interfaces;
using Student.Domain.Entities;

public interface IStudentService
{
    Task<Student> CreateAsync(string fullName, string email, CancellationToken ct = default);
    Task<Student?> GetByEmailAsync(string email, CancellationToken ct = default);

    Task<Student?> GetAsync(long id, CancellationToken ct = default);
    Task<List<Student>> ListAsync(CancellationToken ct = default);
    Task<bool> UpdateAsync(long id, string fullName, string email, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
