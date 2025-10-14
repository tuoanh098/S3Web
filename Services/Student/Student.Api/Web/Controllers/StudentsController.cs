namespace Student.Api.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Student.Api.Web.Requests;
using Student.Api.Web.Responses;
using Student.Application.Interfaces;
using Student.Domain.Entities;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(IStudentService service, ILogger<StudentsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _service.ListAsync();
        var dto = list.Select(s => new StudentDto(s.Id, s.FullName, s.Email, s.JoinedAt)).ToList();
        return Ok(dto);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id)
    {
        var s = await _service.GetAsync(id);
        if (s is null) return NotFound();
        return Ok(new StudentDto(s.Id, s.FullName, s.Email, s.JoinedAt));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest req)
    {
        var s = await _service.CreateAsync(req.FullName, req.Email);
        return CreatedAtAction(nameof(Get), new { id = s.Id }, new StudentDto(s.Id, s.FullName, s.Email, s.JoinedAt));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] CreateStudentRequest req)
    {
        var ok = await _service.UpdateAsync(id, req.FullName, req.Email);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }
}
