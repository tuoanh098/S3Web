public static class StudentMapper
{
    public static StudentProfileDto ToDto(Student.Domain.Entities.Student s, string? email)
    {
        return new StudentProfileDto
        {
            UserId = s.UserId,
            FirstName = s.FirstName,
            LastName = s.LastName,
            Birthday = s.Birthday,
            Gender = s.Gender,
            Nation = s.Nation,
            Email = email,
            Mobile = s.Mobile,
            Parent = s.Parent,
            Bio = s.Bio,
            JoinedAt = s.JoinedAt
        };
    }
}
