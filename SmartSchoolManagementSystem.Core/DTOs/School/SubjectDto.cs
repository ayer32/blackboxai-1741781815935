namespace SmartSchoolManagementSystem.Core.DTOs.School;

public class SubjectDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Credits { get; set; }
    public Guid TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public List<string> AssignedClasses { get; set; } = new();
    public int TotalStudents { get; set; }
}

public class CreateSubjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Credits { get; set; }
    public Guid TeacherId { get; set; }
    public List<Guid> ClassIds { get; set; } = new();
}

public class UpdateSubjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Credits { get; set; }
    public Guid TeacherId { get; set; }
    public List<Guid> ClassIds { get; set; } = new();
}

public class SubjectSummaryDto
{
    public int TotalSubjects { get; set; }
    public Dictionary<int, int> SubjectsByCredits { get; set; } = new();
    public Dictionary<string, int> SubjectsByTeacher { get; set; } = new();
    public double AverageStudentsPerSubject { get; set; }
    public List<SubjectEnrollmentDto> TopEnrolledSubjects { get; set; } = new();
}

public class SubjectEnrollmentDto
{
    public Guid SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public int EnrolledStudents { get; set; }
    public int AssignedClasses { get; set; }
}
