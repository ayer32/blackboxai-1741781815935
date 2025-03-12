namespace SmartSchoolManagementSystem.Core.DTOs.School;

public class StudentDto : BaseDto
{
    public string StudentId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public DateTime DateOfBirth { get; set; }
    public int Age => DateTime.Today.Year - DateOfBirth.Year;
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
}

public class CreateStudentDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public string? PhotoUrl { get; set; }
}

public class UpdateStudentDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public string? PhotoUrl { get; set; }
}

public class StudentSummaryDto
{
    public int TotalStudents { get; set; }
    public Dictionary<string, int> StudentsByGender { get; set; } = new();
    public Dictionary<string, int> StudentsByClass { get; set; } = new();
    public double AverageAttendanceRate { get; set; }
    public int StudentsWithOverdueBooks { get; set; }
}
