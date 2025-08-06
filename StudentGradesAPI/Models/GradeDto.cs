namespace StudentGradesAPI.Models;

public class GradeDto
{
    public string CourseName { get; set; } = string.Empty;
    public int Grade { get; set; }
}


public class StudentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<GradeDto> Grades { get; set; } = new();
}