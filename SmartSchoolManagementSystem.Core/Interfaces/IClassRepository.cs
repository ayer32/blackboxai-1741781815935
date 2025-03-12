using SmartSchoolManagementSystem.Core.Entities.School;

namespace SmartSchoolManagementSystem.Core.Interfaces;

public interface IClassRepository : IRepository<Class>
{
    Task<IReadOnlyList<Class>> GetClassesByAcademicYearAsync(int academicYear);
    Task<IReadOnlyList<Student>> GetClassStudentsAsync(Guid classId);
    Task<IReadOnlyList<Subject>> GetClassSubjectsAsync(Guid classId);
    Task<int> GetStudentCountAsync(Guid classId);
    Task<Teacher?> GetClassTeacherAsync(Guid classId);
    Task<bool> IsClassNameUniqueAsync(string name, string section, int academicYear, Guid? excludeId = null);
    Task<IReadOnlyList<Class>> SearchClassesAsync(string searchTerm);
    Task<Dictionary<Guid, int>> GetAttendanceStatisticsAsync(Guid classId, DateTime date);
}
