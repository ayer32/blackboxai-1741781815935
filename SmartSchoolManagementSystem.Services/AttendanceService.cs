using AutoMapper;
using SmartSchoolManagementSystem.Core.DTOs.School;
using SmartSchoolManagementSystem.Core.Entities.School;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Services;

public class AttendanceService : BaseService<Attendance, AttendanceDto, CreateAttendanceDto, UpdateAttendanceDto>, IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IClassRepository _classRepository;

    public AttendanceService(
        IAttendanceRepository attendanceRepository,
        IStudentRepository studentRepository,
        IClassRepository classRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper) : base(attendanceRepository, unitOfWork, mapper)
    {
        _attendanceRepository = attendanceRepository;
        _studentRepository = studentRepository;
        _classRepository = classRepository;
    }

    public async Task<IReadOnlyList<AttendanceDto>> GetAttendanceByDateAsync(DateTime date)
    {
        var attendance = await _attendanceRepository.GetAttendanceByDateAsync(date);
        return _mapper.Map<IReadOnlyList<AttendanceDto>>(attendance);
    }

    public async Task<IReadOnlyList<AttendanceDto>> GetAttendanceByStudentAsync(Guid studentId, DateTime startDate, DateTime endDate)
    {
        var attendance = await _attendanceRepository.GetAttendanceByStudentAsync(studentId, startDate, endDate);
        return _mapper.Map<IReadOnlyList<AttendanceDto>>(attendance);
    }

    public async Task<IReadOnlyList<AttendanceDto>> GetAttendanceByClassAsync(Guid classId, DateTime date)
    {
        var attendance = await _attendanceRepository.GetAttendanceByClassAsync(classId, date);
        return _mapper.Map<IReadOnlyList<AttendanceDto>>(attendance);
    }

    public async Task<double> GetStudentAttendancePercentageAsync(Guid studentId, DateTime startDate, DateTime endDate)
    {
        return await _attendanceRepository.GetStudentAttendancePercentageAsync(studentId, startDate, endDate);
    }

    public async Task<Dictionary<Guid, double>> GetClassAttendancePercentageAsync(Guid classId, DateTime startDate, DateTime endDate)
    {
        return await _attendanceRepository.GetClassAttendancePercentageAsync(classId, startDate, endDate);
    }

    public async Task<bool> HasAttendanceBeenMarkedAsync(Guid studentId, DateTime date)
    {
        return await _attendanceRepository.HasAttendanceBeenMarkedAsync(studentId, date);
    }

    public async Task<IReadOnlyList<StudentDto>> GetAbsentStudentsAsync(DateTime date)
    {
        var students = await _attendanceRepository.GetAbsentStudentsAsync(date);
        return _mapper.Map<IReadOnlyList<StudentDto>>(students);
    }

    public async Task<AttendanceSummaryDto> GetAttendanceSummaryAsync(DateTime date)
    {
        var classes = await _classRepository.GetAllAsync();
        var attendanceStats = new Dictionary<string, AttendanceStatDto>();

        foreach (var @class in classes)
        {
            var classAttendance = await _attendanceRepository.GetAttendanceByClassAsync(@class.Id, date);
            var totalStudents = classAttendance.Count;
            var presentStudents = classAttendance.Count(a => a.IsPresent);

            attendanceStats.Add($"{@class.Name} - {@class.Section}", new AttendanceStatDto
            {
                TotalStudents = totalStudents,
                PresentStudents = presentStudents,
                AbsentStudents = totalStudents - presentStudents,
                AttendancePercentage = totalStudents > 0 
                    ? (double)presentStudents / totalStudents * 100 
                    : 0
            });
        }

        var allAttendance = await _attendanceRepository.GetAttendanceByDateAsync(date);
        var summary = new AttendanceSummaryDto
        {
            Date = date,
            TotalStudents = allAttendance.Count,
            PresentStudents = allAttendance.Count(a => a.IsPresent),
            AbsentStudents = allAttendance.Count(a => !a.IsPresent),
            AttendancePercentage = allAttendance.Any()
                ? (double)allAttendance.Count(a => a.IsPresent) / allAttendance.Count * 100
                : 0,
            AttendanceByClass = attendanceStats
        };

        return summary;
    }

    public async Task<MonthlyAttendanceDto> GetMonthlyAttendanceAsync(int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var workingDays = 0;
        var dailyAttendance = new List<DailyAttendanceDto>();

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
            {
                workingDays++;
                var attendance = await _attendanceRepository.GetAttendanceByDateAsync(date);
                var totalStudents = attendance.Count;
                var presentStudents = attendance.Count(a => a.IsPresent);

                dailyAttendance.Add(new DailyAttendanceDto
                {
                    Date = date,
                    PresentCount = presentStudents,
                    AbsentCount = totalStudents - presentStudents,
                    AttendancePercentage = totalStudents > 0
                        ? (double)presentStudents / totalStudents * 100
                        : 0
                });
            }
        }

        return new MonthlyAttendanceDto
        {
            Year = year,
            Month = month,
            WorkingDays = workingDays,
            AverageAttendance = dailyAttendance.Any()
                ? dailyAttendance.Average(d => d.AttendancePercentage)
                : 0,
            DailyAttendance = dailyAttendance
        };
    }

    public async Task BulkCreateAttendanceAsync(BulkAttendanceDto bulkAttendanceDto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var attendances = new List<Attendance>();
            foreach (var studentAttendance in bulkAttendanceDto.StudentAttendances)
            {
                if (await HasAttendanceBeenMarkedAsync(studentAttendance.StudentId, bulkAttendanceDto.Date))
                    continue;

                attendances.Add(new Attendance
                {
                    StudentId = studentAttendance.StudentId,
                    Date = bulkAttendanceDto.Date,
                    IsPresent = studentAttendance.IsPresent,
                    Remarks = studentAttendance.Remarks
                });
            }

            await _attendanceRepository.BulkCreateAttendanceAsync(attendances);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<Dictionary<string, AttendanceStatDto>> GetAttendanceStatsByClassAsync(DateTime date)
    {
        var classes = await _classRepository.GetAllAsync();
        var stats = new Dictionary<string, AttendanceStatDto>();

        foreach (var @class in classes)
        {
            var attendance = await _attendanceRepository.GetAttendanceByClassAsync(@class.Id, date);
            var totalStudents = attendance.Count;
            var presentStudents = attendance.Count(a => a.IsPresent);

            stats.Add($"{@class.Name} - {@class.Section}", new AttendanceStatDto
            {
                TotalStudents = totalStudents,
                PresentStudents = presentStudents,
                AbsentStudents = totalStudents - presentStudents,
                AttendancePercentage = totalStudents > 0
                    ? (double)presentStudents / totalStudents * 100
                    : 0
            });
        }

        return stats;
    }

    public async Task<bool> UpdateBulkAttendanceAsync(BulkAttendanceDto bulkAttendanceDto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            foreach (var studentAttendance in bulkAttendanceDto.StudentAttendances)
            {
                var attendance = await _attendanceRepository.FindAsync(a =>
                    a.StudentId == studentAttendance.StudentId &&
                    a.Date.Date == bulkAttendanceDto.Date.Date);

                var existingAttendance = attendance.FirstOrDefault();
                if (existingAttendance != null)
                {
                    existingAttendance.IsPresent = studentAttendance.IsPresent;
                    existingAttendance.Remarks = studentAttendance.Remarks;
                    await _attendanceRepository.UpdateAsync(existingAttendance);
                }
                else
                {
                    await _attendanceRepository.AddAsync(new Attendance
                    {
                        StudentId = studentAttendance.StudentId,
                        Date = bulkAttendanceDto.Date,
                        IsPresent = studentAttendance.IsPresent,
                        Remarks = studentAttendance.Remarks
                    });
                }
            }

            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            return false;
        }
    }
}
