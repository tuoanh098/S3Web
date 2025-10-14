namespace Student.Infrastructure.Services;
using Student.Application.Interfaces;
using Student.Domain.Entities;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _repo;
    public StudentService(IStudentRepository repo) => _repo = repo;

    public async Task<Student?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return await _repo.GetByEmailAsync(email.Trim());
    }

    public async Task<Student> CreateAsync(string fullName, string email, CancellationToken ct = default)
    {
        var s = new Student
        {
            FullName = fullName,
            Email = email,
            JoinedAt = DateTime.UtcNow
        };
        return await _repo.AddAsync(s, ct);
    }

    public async Task<Student?> GetAsync(long id, CancellationToken ct = default) =>
        await _repo.GetAsync(id, ct);

    public async Task<List<Student>> ListAsync(CancellationToken ct = default) =>
        await _repo.ListAsync(ct);

    public async Task<bool> UpdateAsync(long id, string fullName, string email, CancellationToken ct = default)
    {
        var s = await _repo.GetAsync(id, ct);
        if (s is null) return false;
        s.FullName = fullName;
        s.Email = email;
        return await _repo.UpdateAsync(s, ct);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default) =>
        await _repo.DeleteAsync(id, ct);
}
