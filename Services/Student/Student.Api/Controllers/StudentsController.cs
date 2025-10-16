using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Student.Application.Dto;
using Student.Application.Services;

namespace Student.Api.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IConfiguration _configuration;

        public StudentsController(IStudentService studentService, IConfiguration configuration)
        {
            _studentService = studentService;
            _configuration = configuration;
        }

        [HttpGet("me/{userId}")]
        public async Task<IActionResult> GetByUserIdInternal(string userId)
        {
            // Validate internal token header
            var expected = _configuration["Internal:ServiceToken"];
            var provided = Request.Headers["X-Internal-Token"].FirstOrDefault();

            if (string.IsNullOrEmpty(expected) || !string.Equals(expected, provided, StringComparison.Ordinal))
            {
                // Unauthorized for callers that don't provide the correct internal token
                return Unauthorized();
            }

            var profile = await _studentService.GetByUserIdAsync(userId);
            if (profile is null) return NotFound(new { error = "profile_not_found" });
            return Ok(profile);
        }
    }
}
