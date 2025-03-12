using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Entities.School;

namespace SmartSchoolManagementSystem.Core.Interfaces.Services;

public interface ISubjectService : IBaseService<Subject, SubjectDto, CreateSubjectDto, UpdateSubjectDto>
{
    Task<SubjectDto> GetSubjectByCodeAsync(string code);
    Task<IReadOnlyList<SubjectDto>> GetSubjectsByTeacherAsync(Guid teacherId);
    Task<IReadOnlyList<SubjectDto>> GetSubjectsByClassAsync(Guid classId);
    Task<bool> IsSubjectCodeUniqueAsync(string code, Guid? excludeId = null);
    Task<IReadOnlyList<SubjectDto>> SearchSubjectsAsync(string searchTerm);
    Task<IReadOnlyList<ClassDto>> GetSubjectClassesAsync(Guid subjectId);
    Task<int> GetSubjectStudentCountAsync(Guid subjectId);
    Task<SubjectSummaryDto> GetSubjectSummaryAsync();
    Task<bool> AssignTeacherToSubjectAsync(Guid subjectId, Guid teacherId);
    Task<bool> RemoveTeacherFromSubjectAsync(Guid subjectId);
    Task<IReadOnlyList<SubjectEnrollmentDto>> GetTopEnrolledSubjectsAsync(int count = 10);
    Task<bool> IsTeacherQualifiedForSubjectAsync(Guid teacherId, Guid subjectId);
}
