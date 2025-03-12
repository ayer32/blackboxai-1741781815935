namespace SmartSchoolManagementSystem.Core.DTOs.School;

public class ClassDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string FullName => $"{Name} - {Section}";
    public int AcademicYear { get; set; }
    public Guid TeacherId { get; set; }
    public string ClassTeacherName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public List<SubjectDto> Subjects { get; set; } = new();
}

public class CreateClassDto
{
    public string Name { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public int AcademicYear { get; set; }
    public Guid TeacherId { get; set; }
    public List<Guid> SubjectIds { get; set; } = new();
}

public class UpdateClassDto
{
    public string Name { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public Guid TeacherId { get; set; }
    public List<Guid> SubjectIds { get; set; } = new();
}

public class ClassSummaryDto
{
    public int TotalClasses { get; set; }
    public Dictionary<int, int> ClassesByAcademicYear { get; set; } = new();
    public Dictionary<string, int> StudentsByClass { get; set; } = new();
    public double AverageStudentsPerClass { get; set; }
    public Dictionary<string, double> AttendanceByClass { get; set; } = new();
}

public class ClassAttendanceDto
{
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int TotalStudents { get; set; }
    public int PresentStudents { get; set; }
    public int AbsentStudents { get; set; }
    public double AttendancePercentage { get; set; }
}
