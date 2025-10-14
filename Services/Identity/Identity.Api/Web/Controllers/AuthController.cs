using System;
using System.Net;
using System.Threading.Tasks;
using Identity.Api.Web.Requests;
using Identity.Application.Auth;
using Identity.Application.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Identity.Api.Web.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _users;
    private readonly ITokenService _tokens;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<IdentityUser> users,
        ITokenService tokens,
        IEmailSender emailSender,
        IConfiguration configuration,
        IWebHostEnvironment env,
        ILogger<AuthController> logger)
    {
        _users = users;
        _tokens = tokens;
        _emailSender = emailSender;
        _configuration = configuration;
        _env = env;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _users.FindByEmailAsync(req.Email) ?? await _users.FindByNameAsync(req.Email);
        if (user is null || !await _users.CheckPasswordAsync(user, req.Password)) return Unauthorized();

        var roles = await _users.GetRolesAsync(user);
        var access = _tokens.CreateAccessToken(user, roles);
        var (rt, exp) = await _tokens.IssueRefreshAsync(user.Id);
        SetRefreshCookie(rt, exp);

        return Ok(new { access_token = access, token_type = "Bearer", roles });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var rt = Request.Cookies["rt"];
        if (string.IsNullOrEmpty(rt)) return Unauthorized();

        var user = await _tokens.ValidateRefreshAsync(rt);
        if (user is null) return Unauthorized();

        var rotated = await _tokens.RotateAsync(rt);
        if (rotated is null) return Unauthorized();

        var roles = await _users.GetRolesAsync(user);
        var access = _tokens.CreateAccessToken(user, roles);
        SetRefreshCookie(rotated.Value.newToken, rotated.Value.expires);

        return Ok(new { access_token = access, token_type = "Bearer" });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var rt = Request.Cookies["rt"];
        if (!string.IsNullOrEmpty(rt))
        {
            await _tokens.RevokeAsync(rt, "logout");
            Response.Cookies.Delete("rt", BuildCookieOptions(DateTime.UtcNow.AddDays(-1)));
        }
        return NoContent();
    }

    /// <summary>
    /// Forgot password: generate reset token, send email, return 202.
    /// Always returns 202 to avoid user enumeration. In Development returns token+url in response for debugging.
    /// </summary>
    [HttpPost("forgot")]
    public async Task<IActionResult> Forgot([FromBody] ForgotPasswordRequest req)
    {
        if (req is null || string.IsNullOrWhiteSpace(req.Email))
            return BadRequest(new { error = "Email is required." });

        try
        {
            var email = req.Email.Trim();
            var user = await _users.FindByEmailAsync(email);
            if (user is null)
            {
                // Always return Accepted to avoid leaking whether email exists
                _logger.LogInformation("Forgot requested for non-existing email {Email}", email);
                return Accepted(new { sent = true });
            }

            // Generate token (requires AddDefaultTokenProviders)
            var token = await _users.GeneratePasswordResetTokenAsync(user);

            // Build frontend reset URL
            var feBase = _configuration["Frontend:BaseUrl"]?.TrimEnd('/') ?? "http://localhost:5173";
            var encodedToken = WebUtility.UrlEncode(token);
            var encodedEmail = WebUtility.UrlEncode(user.Email ?? "");
            var resetUrl = $"{feBase}/reset-password?email={encodedEmail}&token={encodedToken}";

            // Compose an email (simple HTML)
            var html = $@"
                <p>Dear {WebUtility.HtmlEncode(user.UserName ?? user.Email ?? "")},</p>
                <p>We received a request to reset your RAS password. Click the button below to reset it.</p>
                <p style='margin:18px 0;'><a href='{resetUrl}' style='background:#3B3A8A;color:white;padding:10px 16px;border-radius:6px;text-decoration:none;'>Reset password</a></p>
                <p>If the button doesn't work, copy and paste this URL into your browser:</p>
                <p><code>{WebUtility.HtmlEncode(resetUrl)}</code></p>
                <p>If you didn't request this, you can ignore this email.</p>
                <hr/><p>Thanks — RAS Support</p>
            ";

            // Try to send — don't fail the flow if sending errors; log the error
            try
            {
                await _emailSender.SendAsync(user.Email!, "Reset your RAS password", html);
                _logger.LogInformation("Password reset email sent to {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
                // swallow: we still return Accepted to client to avoid enumeration
            }

            // Dev helper: expose token and url for testing (remove in production)
            if (_env.IsDevelopment())
            {
                return Accepted(new { sent = true, debug = new { token, resetUrl } });
            }

            return Accepted(new { sent = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error in Forgot endpoint for {Email}", req.Email);
            // Generic response so we don't leak internals
            return Problem(detail: "An error occurred while processing the request.");
        }
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset([FromBody] ResetPasswordRequest req)
    {
        if (req is null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Token) || string.IsNullOrWhiteSpace(req.NewPassword))
            return BadRequest(new { error = "Email, token and newPassword are required." });

        try
        {
            var email = req.Email.Trim();
            var user = await _users.FindByEmailAsync(email);
            if (user is null)
            {
                // don't reveal existence — return BadRequest to indicate failure similar to invalid token
                return BadRequest(new { error = "Invalid token or user." });
            }

            // IMPORTANT: DO NOT UrlDecode the token here.
            // The browser will already decode query params; use req.Token as-is.
            var result = await _users.ResetPasswordAsync(user, req.Token, req.NewPassword);

            if (!result.Succeeded)
            {
                // map errors to a friendly response but don't leak specifics
                var errors = result.Errors.Select(e => e.Description).ToArray();
                _logger.LogInformation("Password reset failed for {Email}: {Errors}", user.Email, string.Join("; ", errors));
                return BadRequest(new { error = "Invalid token or password requirements not met.", details = errors });
            }

            _logger.LogInformation("Password reset successful for {Email}", user.Email);
            return NoContent(); // 204 -> success, nothing to return
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error in Reset endpoint for {Email}", req?.Email);
            return Problem(detail: "An error occurred while processing the request.");
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        // Get the current user from the HttpContext.
        // GetUserAsync will read the user's ID from the token's claims.
        var user = await _users.GetUserAsync(User);
        if (user is null)
        {
            // This should not happen if [Authorize] is working, but it's good practice.
            return NotFound();
        }

        // Get the roles associated with the user.
        var roles = await _users.GetRolesAsync(user);

        // Return the user's information.
        return Ok(new
        {
            email = user.Email,
            userName = user.UserName,
            roles = roles
        });
    }

    private void SetRefreshCookie(string token, DateTime expiresUtc) =>
        Response.Cookies.Append("rt", token, BuildCookieOptions(expiresUtc));

    private static CookieOptions BuildCookieOptions(DateTime expiresUtc) => new()
    {
        HttpOnly = true,
        Secure = true,                 // set to false only for http localhost
        SameSite = SameSiteMode.None,
        Expires = expiresUtc
    };
}
