namespace SmartSchoolManagementSystem.Core.Entities.School;

public class Teacher : BaseEntity
{
    public string TeacherId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public ICollection<Class> Classes { get; set; } = new List<Class>();
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
