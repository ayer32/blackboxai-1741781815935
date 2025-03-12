using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Entities.School;

namespace SmartSchoolManagementSystem.Core.Interfaces.Services;

public interface IAttendanceService : IBaseService<Attendance, AttendanceDto, CreateAttendanceDto, UpdateAttendanceDto>
{
    Task<IReadOnlyList<AttendanceDto>> GetAttendanceByDateAsync(DateTime date);
    Task<IReadOnlyList<AttendanceDto>> GetAttendanceByStudentAsync(Guid studentId, DateTime startDate, DateTime endDate);
    Task<IReadOnlyList<AttendanceDto>> GetAttendanceByClassAsync(Guid classId, DateTime date);
    Task<double> GetStudentAttendancePercentageAsync(Guid studentId, DateTime startDate, DateTime endDate);
    Task<Dictionary<Guid, double>> GetClassAttendancePercentageAsync(Guid classId, DateTime startDate, DateTime endDate);
    Task<bool> HasAttendanceBeenMarkedAsync(Guid studentId, DateTime date);
    Task<IReadOnlyList<StudentDto>> GetAbsentStudentsAsync(DateTime date);
    Task<AttendanceSummaryDto> GetAttendanceSummaryAsync(DateTime date);
    Task<MonthlyAttendanceDto> GetMonthlyAttendanceAsync(int year, int month);
    Task BulkCreateAttendanceAsync(BulkAttendanceDto bulkAttendanceDto);
    Task<Dictionary<string, AttendanceStatDto>> GetAttendanceStatsByClassAsync(DateTime date);
    Task<bool> UpdateBulkAttendanceAsync(BulkAttendanceDto bulkAttendanceDto);
}
