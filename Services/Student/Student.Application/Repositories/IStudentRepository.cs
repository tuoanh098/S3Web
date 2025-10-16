namespace Student.Application.Repositories
{
    public interface IStudentRepository
    {
        Task<Domain.Entities.Student?> GetByUserIdAsync(string userId);
        Task<Domain.Entities.Student?> GetByIdAsync(Guid id);
        Task AddAsync(Domain.Entities.Student student);
        Task UpdateAsync(Domain.Entities.Student student);
    }
}
