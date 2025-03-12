using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Entities.School;

namespace SmartSchoolManagementSystem.Core.Interfaces.Services;

public interface IClassService : IBaseService<Class, ClassDto, CreateClassDto, UpdateClassDto>
{
    Task<IReadOnlyList<ClassDto>> GetClassesByAcademicYearAsync(int academicYear);
    Task<IReadOnlyList<StudentDto>> GetClassStudentsAsync(Guid classId);
    Task<IReadOnlyList<SubjectDto>> GetClassSubjectsAsync(Guid classId);
    Task<int> GetStudentCountAsync(Guid classId);
    Task<TeacherDto?> GetClassTeacherAsync(Guid classId);
    Task<bool> IsClassNameUniqueAsync(string name, string section, int academicYear, Guid? excludeId = null);
    Task<IReadOnlyList<ClassDto>> SearchClassesAsync(string searchTerm);
    Task<Dictionary<Guid, int>> GetAttendanceStatisticsAsync(Guid classId, DateTime date);
    Task<ClassSummaryDto> GetClassSummaryAsync(int? academicYear = null);
    Task<bool> AssignSubjectToClassAsync(Guid classId, Guid subjectId);
    Task<bool> RemoveSubjectFromClassAsync(Guid classId, Guid subjectId);
    Task<bool> PromoteStudentsAsync(Guid sourceClassId, Guid targetClassId);
    Task<ClassAttendanceDto> GetClassAttendanceAsync(Guid classId, DateTime date);
}
