using Student.Application.Dto;


public interface IStudentService
{
    Task<StudentProfileDto?> GetByUserIdAsync(string userId);
    Task<bool> UpdateProfileAsync(string userId, StudentUpdateDto update);
}