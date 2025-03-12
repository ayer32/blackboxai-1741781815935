namespace SmartSchoolManagementSystem.Core.DTOs.School;

public class AttendanceDto : BaseDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentClass { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool IsPresent { get; set; }
    public string? Remarks { get; set; }
}

public class CreateAttendanceDto
{
    public Guid StudentId { get; set; }
    public DateTime Date { get; set; }
    public bool IsPresent { get; set; }
    public string? Remarks { get; set; }
}

public class UpdateAttendanceDto
{
    public bool IsPresent { get; set; }
    public string? Remarks { get; set; }
}

public class BulkAttendanceDto
{
    public DateTime Date { get; set; }
    public Guid ClassId { get; set; }
    public List<StudentAttendanceDto> StudentAttendances { get; set; } = new();
}

public class StudentAttendanceDto
{
    public Guid StudentId { get; set; }
    public bool IsPresent { get; set; }
    public string? Remarks { get; set; }
}

public class AttendanceSummaryDto
{
    public DateTime Date { get; set; }
    public int TotalStudents { get; set; }
    public int PresentStudents { get; set; }
    public int AbsentStudents { get; set; }
    public double AttendancePercentage { get; set; }
    public Dictionary<string, AttendanceStatDto> AttendanceByClass { get; set; } = new();
}

public class AttendanceStatDto
{
    public int TotalStudents { get; set; }
    public int PresentStudents { get; set; }
    public int AbsentStudents { get; set; }
    public double AttendancePercentage { get; set; }
}

public class MonthlyAttendanceDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int WorkingDays { get; set; }
    public double AverageAttendance { get; set; }
    public List<DailyAttendanceDto> DailyAttendance { get; set; } = new();
}

public class DailyAttendanceDto
{
    public DateTime Date { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public double AttendancePercentage { get; set; }
}
