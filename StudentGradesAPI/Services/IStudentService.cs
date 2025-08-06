using StudentGradesAPI.Models;

namespace StudentGradesAPI.Services;

public interface IStudentService
{
    Task<StudentDto?> GetStudentByIdAsync(int id);
}
