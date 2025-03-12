using SmartSchoolManagementSystem.Core.Entities.School;

namespace SmartSchoolManagementSystem.Core.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetStudentByStudentIdAsync(string studentId);
    Task<IReadOnlyList<Student>> GetStudentsByClassAsync(Guid classId);
    Task<IReadOnlyList<Student>> SearchStudentsAsync(string searchTerm);
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null);
    Task<bool> IsStudentIdUniqueAsync(string studentId, Guid? excludeId = null);
    Task<IReadOnlyList<Student>> GetStudentsWithOverdueBooksAsync();
    Task<IReadOnlyList<BookLending>> GetStudentLendingHistoryAsync(Guid studentId);
    Task<IReadOnlyList<Attendance>> GetStudentAttendanceHistoryAsync(Guid studentId, DateTime startDate, DateTime endDate);
}
