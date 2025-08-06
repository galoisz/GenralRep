using Microsoft.AspNetCore.Mvc;
using StudentGradesAPI.Models;
using StudentGradesAPI.Services;

namespace StudentGradesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetStudent(int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student == null)
        {
            return NotFound(new { Message = "Student not found" });
        }

        return Ok(student);
    }
}
