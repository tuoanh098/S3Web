using Microsoft.EntityFrameworkCore;
using Student.Application.Repositories;

namespace Student.Infrastructure.Repositories
{
    public sealed class StudentRepository : IStudentRepository
    {
        private readonly StudentDbContext _db;
        public StudentRepository(StudentDbContext db) => _db = db;

        public async Task<Student.Domain.Entities.Student?> GetByUserIdAsync(string userId) =>
            await _db.Students.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);

        public async Task<Student.Domain.Entities.Student?> GetByIdAsync(Guid id) =>
            await _db.Students.FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(Student.Domain.Entities.Student s)
        {
            _db.Students.Add(s);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student.Domain.Entities.Student s)
        {
            _db.Students.Update(s);
            await _db.SaveChangesAsync();
        }
    }
}
