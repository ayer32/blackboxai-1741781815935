using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Entities.School;

namespace SmartSchoolManagementSystem.Core.Interfaces.Services;

public interface ITeacherService : IBaseService<Teacher, TeacherDto, CreateTeacherDto, UpdateTeacherDto>
{
    Task<TeacherDto> GetTeacherByTeacherIdAsync(string teacherId);
    Task<IReadOnlyList<TeacherDto>> SearchTeachersAsync(string searchTerm);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null);
    Task<bool> IsTeacherIdUniqueAsync(string teacherId, Guid? excludeId = null);
    Task<IReadOnlyList<ClassDto>> GetTeacherClassesAsync(Guid teacherId);
    Task<IReadOnlyList<SubjectDto>> GetTeacherSubjectsAsync(Guid teacherId);
    Task<TeacherSummaryDto> GetTeacherSummaryAsync();
    Task<string> GenerateTeacherIdAsync();
    Task<bool> AssignClassAsync(Guid teacherId, Guid classId);
    Task<bool> AssignSubjectAsync(Guid teacherId, Guid subjectId);
    Task<bool> RemoveClassAssignmentAsync(Guid teacherId, Guid classId);
    Task<bool> RemoveSubjectAssignmentAsync(Guid teacherId, Guid subjectId);
    Task<bool> IsTeacherQualifiedForSubjectAsync(Guid teacherId, Guid subjectId);
    Task<int> GetTeacherWorkloadAsync(Guid teacherId);
}
