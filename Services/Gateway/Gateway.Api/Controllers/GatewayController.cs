using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gateway.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class GatewayController : ControllerBase
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _config;

        public GatewayController(IHttpClientFactory httpFactory, IConfiguration config)
        {
            _http = httpFactory;
            _config = config;
        }

        [HttpPost("gateway/login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
                return BadRequest(new { error = "invalid_payload" });

            var identityClient = _http.CreateClient("identity");

            var resp = await identityClient.PostAsJsonAsync("/auth/login", login);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await resp.Content.ReadAsStringAsync();
                return StatusCode((int)resp.StatusCode, new { error = "identity_error", body });
            }

            var identityResult = await resp.Content.ReadFromJsonAsync<IdentityLoginResponse?>();
            if (identityResult == null) return StatusCode(500, new { error = "invalid_identity_response" });

            // optionally fetch student profile using internal token (if you set it up)
            object? profile = null;
            var userId = identityResult.user?.id;
            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    var studentClient = _http.CreateClient("student");
                    var req = new HttpRequestMessage(HttpMethod.Get, $"/api/students/me/{Uri.EscapeDataString(userId)}");
                    req.Headers.Add("X-Internal-Token", _config["Internal:ServiceToken"] ?? "");
                    var sresp = await studentClient.SendAsync(req);
                    if (sresp.IsSuccessStatusCode)
                    {
                        profile = await sresp.Content.ReadFromJsonAsync<object?>();
                    }
                }
                catch
                {
                    profile = null;
                }
            }

            var aggregated = new
            {
                access_token = identityResult.access_token,
                refresh_token = identityResult.refresh_token,
                user = identityResult.user,
                profile = profile
            };

            return Ok(aggregated);
        }

        [HttpGet("gateway/profile")]
        public async Task<IActionResult> Profile()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader))
                return Unauthorized(new { error = "missing_authorization" });

            var studentClient = _http.CreateClient("student");
            studentClient.DefaultRequestHeaders.Remove("Authorization");
            studentClient.DefaultRequestHeaders.Add("Authorization", authHeader);

            var sresp = await studentClient.GetAsync("/api/students/me");
            var content = await sresp.Content.ReadAsStringAsync();
            if (!sresp.IsSuccessStatusCode)
                return StatusCode((int)sresp.StatusCode, content);

            return Content(content, "application/json");
        }

        [HttpPost("gateway/refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest(new { error = "missing_refresh_token" });

            var identityClient = _http.CreateClient("identity");
            var resp = await identityClient.PostAsJsonAsync("/auth/refresh", new { RefreshToken = dto.RefreshToken });

            var text = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                return StatusCode((int)resp.StatusCode, text);
            }

            // forward response (likely JSON with access_token and refresh_token)
            return Content(text, "application/json");
        }

        [HttpPost("gateway/logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest(new { error = "missing_refresh_token" });

            var identityClient = _http.CreateClient("identity");
            var resp = await identityClient.PostAsJsonAsync("/auth/logout", new { RefreshToken = dto.RefreshToken });

            if (!resp.IsSuccessStatusCode)
            {
                var text = await resp.Content.ReadAsStringAsync();
                return StatusCode((int)resp.StatusCode, text);
            }

            return NoContent();
        }

        public record LoginDto(string Username, string Password);
        public record RefreshDto(string RefreshToken);
        private record IdentityUserSummary(string id, string email, string username);
        private record IdentityLoginResponse(string access_token, string refresh_token, IdentityUserSummary? user);
    }
}
