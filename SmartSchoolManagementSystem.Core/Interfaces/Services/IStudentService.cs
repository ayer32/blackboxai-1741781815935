using SmartSchoolManagementSystem.Core.DTOs.Library;
using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Entities.School;

namespace SmartSchoolManagementSystem.Core.Interfaces.Services;

public interface IStudentService : IBaseService<Student, StudentDto, CreateStudentDto, UpdateStudentDto>
{
    Task<StudentDto> GetStudentByStudentIdAsync(string studentId);
    Task<IReadOnlyList<StudentDto>> GetStudentsByClassAsync(Guid classId);
    Task<IReadOnlyList<StudentDto>> SearchStudentsAsync(string searchTerm);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null);
    Task<bool> IsStudentIdUniqueAsync(string studentId, Guid? excludeId = null);
    Task<IReadOnlyList<StudentDto>> GetStudentsWithOverdueBooksAsync();
    Task<IReadOnlyList<BookLendingDto>> GetStudentLendingHistoryAsync(Guid studentId);
    Task<IReadOnlyList<AttendanceDto>> GetStudentAttendanceHistoryAsync(Guid studentId, DateTime startDate, DateTime endDate);
    Task<StudentSummaryDto> GetStudentSummaryAsync();
    Task<double> GetStudentAttendancePercentageAsync(Guid studentId, DateTime startDate, DateTime endDate);
    Task<string> GenerateStudentIdAsync();
    Task<bool> TransferStudentToClassAsync(Guid studentId, Guid newClassId);
}
