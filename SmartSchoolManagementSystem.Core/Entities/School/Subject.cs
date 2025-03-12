namespace SmartSchoolManagementSystem.Core.Entities.School;

public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Credits { get; set; }
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;
    public ICollection<Class> Classes { get; set; } = new List<Class>();
}
