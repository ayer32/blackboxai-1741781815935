namespace SmartSchoolManagementSystem.Core.Entities.School;

public class Class : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public int AcademicYear { get; set; }
    public Guid TeacherId { get; set; }
    public Teacher ClassTeacher { get; set; } = null!;
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
