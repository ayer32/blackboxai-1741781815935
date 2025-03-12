namespace SmartSchoolManagementSystem.Core.DTOs.School;

public class TeacherDto : BaseDto
{
    public string TeacherId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public List<ClassDto> Classes { get; set; } = new();
    public List<SubjectDto> Subjects { get; set; } = new();
}

public class CreateTeacherDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
}

public class UpdateTeacherDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
}

public class TeacherSummaryDto
{
    public int TotalTeachers { get; set; }
    public Dictionary<string, int> TeachersBySpecialization { get; set; } = new();
    public Dictionary<string, int> TeachersBySubject { get; set; } = new();
    public int AverageClassesPerTeacher { get; set; }
    public int AverageSubjectsPerTeacher { get; set; }
}
