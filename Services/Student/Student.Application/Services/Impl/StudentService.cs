using Student.Application.Dto;
using Student.Application.Repositories;

namespace Student.Application.Services.Impl
{
    public sealed class StudentService : IStudentService
    {
        private readonly IStudentRepository _repo;
        private readonly IIdentityService _identity;

        public StudentService(IStudentRepository repo, IIdentityService identity)
        {
            _repo = repo;
            _identity = identity;
        }

        public async Task<StudentProfileDto?> GetByUserIdAsync(string userId)
        {
            var s = await _repo.GetByUserIdAsync(userId);
            if (s is null) return null;

            var email = await _identity.GetEmailByUserIdAsync(userId);
            return StudentMapper.ToDto(s, email);
        }

        public async Task<bool> UpdateProfileAsync(string userId, StudentUpdateDto updateDto)
        {
            var s = await _repo.GetByUserIdAsync(userId);
            if (s is null) return false;

            if (updateDto.FirstName is not null) s.FirstName = updateDto.FirstName;
            if (updateDto.LastName is not null) s.LastName = updateDto.LastName;
            if (updateDto.Birthday.HasValue) s.Birthday = updateDto.Birthday;
            if (updateDto.Gender is not null) s.Gender = updateDto.Gender;
            if (updateDto.Nation is not null) s.Nation = updateDto.Nation;
            if (updateDto.Mobile is not null) s.Mobile = updateDto.Mobile;
            if (updateDto.Parent is not null) s.Parent = updateDto.Parent;
            if (updateDto.Bio is not null) s.Bio = updateDto.Bio;

            await _repo.UpdateAsync(s);
            return true;
        }
    }
}
