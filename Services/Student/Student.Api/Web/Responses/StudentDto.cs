namespace Student.Api.Web.Responses;
public sealed record StudentDto(long Id, string FullName, string Email, DateTime JoinedAt);
