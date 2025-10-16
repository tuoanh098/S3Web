namespace Student.Application.Services;

public interface IIdentityService
{    Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken ct = default);
}
