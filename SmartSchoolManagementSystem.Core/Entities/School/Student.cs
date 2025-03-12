namespace SmartSchoolManagementSystem.Core.Entities.School;

public class Student : BaseEntity
{
    public string StudentId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Guid ClassId { get; set; }
    public Class Class { get; set; } = null!;
    public string? PhotoUrl { get; set; }
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<BookLending> BookLendings { get; set; } = new List<BookLending>();
}
