using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Student.Application.Services;

namespace Student.Infrastructure.Services
{
    public sealed class GatewayIdentityService : IIdentityService
    {
        private readonly HttpClient _http;

        public GatewayIdentityService(HttpClient http)
        {
            _http = http;
        }

        // This assumes your gateway has an endpoint like: GET /auth/users/{userId}
        // which returns JSON including "email". Adjust path/response parsing to your API.
        public async Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            try
            {
                var resp = await _http.GetAsync($"/auth/users/{userId}", ct);
                if (!resp.IsSuccessStatusCode) return null;

                // Minimal dynamic parsing — you can define a DTO instead
                var doc = await resp.Content.ReadFromJsonAsync<UserInfoDto?>(cancellationToken: ct);
                return doc?.Email;
            }
            catch
            {
                // log if you have a logger; return null on error to avoid crashing
                return null;
            }
        }

        private class UserInfoDto
        {
            public string? Id { get; set; }
            public string? Email { get; set; }
        }
    }
}
