using Identity.Application.Auth;
using Identity.Application.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository; // service to validate credentials & fetch user
        private readonly IConfiguration _configuration;

        public AuthController(
            ILogger<AuthController> logger,
            ITokenService tokenService,
            IUserRepository userRepository,
            IConfiguration configuration)
        {
            _logger = logger;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { error = "invalid_credentials_payload" });

            // Validate credentials (implement in IUserRepository or user service)
            var user = await _userRepository.ValidateCredentialsAsync(req.Username, req.Password);
            if (user == null)
                return Unauthorized(new { error = "invalid_credentials" });

            // Create access token (JWT) and refresh token as appropriate
            // If you later add role lookup in IUserRepository you can pass real roles here
            var accessToken = _tokenService.CreateAccessToken(user, Enumerable.Empty<string>());

            // Create and persist refresh token
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);

            // Optionally persist refresh token, set cookie, etc. (not calling Student service)
            // Return minimal user info along with tokens (do not call Student service for full profile)
            var response = new
            {
                access_token = accessToken,
                refresh_token = refreshToken,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    username = user.UserName
                }
            };

            return Ok(response);
        }

        // POST /auth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.RefreshToken))
                return BadRequest(new { error = "invalid_refresh_payload" });

            // Option A: validate refresh token and get the user
            var userFromToken = await _tokenService.ValidateRefreshAsync(req.RefreshToken);
            if (userFromToken == null)
            {
                // Option B (alternate flow): try rotating directly (if RotateAsync is used in your design)
                // var rotated = await _tokenService.RotateAsync(req.RefreshToken);
                // if (rotated == null) return Unauthorized(new { error = "invalid_refresh_token" });
                // var newAccess2 = _tokenService.CreateAccessToken(userFromRotated, Enumerable.Empty<string>());
                // return Ok(new { access_token = newAccess2, refresh_token = rotated.Value.newToken });

                return Unauthorized(new { error = "invalid_refresh_token" });
            }

            // At this point, token valid and user found.
            var userId = userFromToken.Id;
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { error = "invalid_refresh_token" });

            // Optionally rotate refresh tokens for better security:
            var rotated = await _tokenService.RotateAsync(req.RefreshToken);
            string newRefresh;
            if (rotated.HasValue)
            {
                newRefresh = rotated.Value.newToken;
            }
            else
            {
                // If rotate failed for some reason, issue a fresh refresh token (fallback)
                newRefresh = await _tokenService.CreateRefreshTokenAsync(userFromToken);
            }

            var newAccess = _tokenService.CreateAccessToken(userFromToken, Enumerable.Empty<string>());

            return Ok(new { access_token = newAccess, refresh_token = newRefresh });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequestDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.RefreshToken))
                return BadRequest(new { error = "invalid_logout_payload" });
            await _tokenService.RevokeAsync(req.RefreshToken, "user_logout");
            return NoContent();
        }
    }
    public record LoginRequestDto(string Username, string Password);
    public record RefreshRequestDto(string RefreshToken);
}
