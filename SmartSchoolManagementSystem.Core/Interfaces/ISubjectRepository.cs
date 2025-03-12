using SmartSchoolManagementSystem.Core.Entities.School;

namespace SmartSchoolManagementSystem.Core.Interfaces;

public interface ISubjectRepository : IRepository<Subject>
{
    Task<Subject?> GetSubjectByCodeAsync(string code);
    Task<IReadOnlyList<Subject>> GetSubjectsByTeacherAsync(Guid teacherId);
    Task<IReadOnlyList<Subject>> GetSubjectsByClassAsync(Guid classId);
    Task<bool> IsSubjectCodeUniqueAsync(string code, Guid? excludeId = null);
    Task<IReadOnlyList<Subject>> SearchSubjectsAsync(string searchTerm);
    Task<IReadOnlyList<Class>> GetSubjectClassesAsync(Guid subjectId);
    Task<int> GetSubjectStudentCountAsync(Guid subjectId);
    Task<bool> IsTeacherQualifiedForSubjectAsync(Guid teacherId, Guid subjectId);
}
