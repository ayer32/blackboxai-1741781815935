using SmartSchoolManagementSystem.Core.Entities.School;

namespace SmartSchoolManagementSystem.Core.Interfaces;

public interface IAttendanceRepository : IRepository<Attendance>
{
    Task<IReadOnlyList<Attendance>> GetAttendanceByDateAsync(DateTime date);
    Task<IReadOnlyList<Attendance>> GetAttendanceByStudentAsync(Guid studentId, DateTime startDate, DateTime endDate);
    Task<IReadOnlyList<Attendance>> GetAttendanceByClassAsync(Guid classId, DateTime date);
    Task<double> GetStudentAttendancePercentageAsync(Guid studentId, DateTime startDate, DateTime endDate);
    Task<Dictionary<Guid, double>> GetClassAttendancePercentageAsync(Guid classId, DateTime startDate, DateTime endDate);
    Task<bool> HasAttendanceBeenMarkedAsync(Guid studentId, DateTime date);
    Task<IReadOnlyList<Student>> GetAbsentStudentsAsync(DateTime date);
    Task BulkCreateAttendanceAsync(IEnumerable<Attendance> attendances);
}
