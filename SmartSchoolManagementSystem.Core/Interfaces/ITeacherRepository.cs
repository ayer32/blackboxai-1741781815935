using SmartSchoolManagementSystem.Core.Entities.School;

namespace SmartSchoolManagementSystem.Core.Interfaces;

public interface ITeacherRepository : IRepository<Teacher>
{
    Task<Teacher?> GetTeacherByTeacherIdAsync(string teacherId);
    Task<IReadOnlyList<Teacher>> SearchTeachersAsync(string searchTerm);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null);
    Task<bool> IsTeacherIdUniqueAsync(string teacherId, Guid? excludeId = null);
    Task<IReadOnlyList<Class>> GetTeacherClassesAsync(Guid teacherId);
    Task<IReadOnlyList<Subject>> GetTeacherSubjectsAsync(Guid teacherId);
    Task<int> GetTeacherClassCountAsync(Guid teacherId);
    Task<int> GetTeacherSubjectCountAsync(Guid teacherId);
}
