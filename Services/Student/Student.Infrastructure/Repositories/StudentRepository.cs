
namespace Student.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Student.Application.Interfaces;
using Student.Domain.Entities;
using Student.Infrastructure.Persistence;

public class StudentRepository : IStudentRepository
{
    private readonly StudentDbContext _db;
    public StudentRepository(StudentDbContext db) => _db = db;

    public async Task<Student> AddAsync(Student s, CancellationToken ct = default)
    {
        _db.Students.Add(s);
        await _db.SaveChangesAsync(ct);
        return s;
    }

    public async Task<Student?> GetAsync(long id, CancellationToken ct = default) =>
        await _db.Students.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<List<Student>> ListAsync(CancellationToken ct = default) =>
        await _db.Students.AsNoTracking().OrderByDescending(s => s.JoinedAt).ToListAsync(ct);

    public async Task<bool> UpdateAsync(Student s, CancellationToken ct = default)
    {
        var exists = await _db.Students.AnyAsync(x => x.Id == s.Id, ct);
        if (!exists) return false;
        _db.Students.Update(s);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var s = await _db.Students.FindAsync(new object[] { id }, ct);
        if (s is null) return false;
        _db.Students.Remove(s);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
