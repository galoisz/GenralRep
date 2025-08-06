using System.Text.Json;
using StudentGradesAPI.Models;

namespace StudentGradesAPI.Services;

public class MockStudentService : IStudentService
{
    private readonly ICacheManagerService _cacheManagerService;
    private readonly string _filePath = "Data/Students.json"; // Path to the JSON file.

    public MockStudentService(ICacheManagerService cacheManagerService)
    {
        _cacheManagerService = cacheManagerService;
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id)
    {
        var cacheKey = $"student:{id}";

        // Check cache first
        var cachedStudent = await _cacheManagerService.GetFromCacheAsync<StudentDto>(cacheKey);
        if (cachedStudent != null)
        {
            return cachedStudent;
        }

        // Load students from the JSON file
        var students = await LoadStudentsFromFileAsync();
        var student = students.FirstOrDefault(s => s.Id == id);
        if (student != null)
        {
            // Cache the data with a 5-minute expiration
            await _cacheManagerService.SetCacheAsync(cacheKey, student, TimeSpan.FromMinutes(5));
        }

        return student;
    }

    private async Task<List<StudentDto>> LoadStudentsFromFileAsync()
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException($"The file {_filePath} was not found.");
        }

        var jsonData = await File.ReadAllTextAsync(_filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        return JsonSerializer.Deserialize<List<StudentDto>>(jsonData, options) ?? new List<StudentDto>();
    }
}
