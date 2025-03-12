using Microsoft.EntityFrameworkCore;
using SmartSchoolManagementSystem.Core.Entities.School;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Infrastructure.Data;

namespace SmartSchoolManagementSystem.Infrastructure.Repositories;

public class AttendanceRepository : BaseRepository<Attendance>, IAttendanceRepository
{
    public AttendanceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Attendance>> GetAttendanceByDateAsync(DateTime date)
    {
        return await _entities
            .Include(a => a.Student)
            .ThenInclude(s => s.Class)
            .Where(a => a.Date.Date == date.Date)
            .OrderBy(a => a.Student.Class.Name)
            .ThenBy(a => a.Student.LastName)
            .ThenBy(a => a.Student.FirstName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Attendance>> GetAttendanceByStudentAsync(Guid studentId, DateTime startDate, DateTime endDate)
    {
        return await _entities
            .Include(a => a.Student)
            .Where(a => 
                a.StudentId == studentId &&
                a.Date.Date >= startDate.Date &&
                a.Date.Date <= endDate.Date)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Attendance>> GetAttendanceByClassAsync(Guid classId, DateTime date)
    {
        return await _entities
            .Include(a => a.Student)
            .Where(a => 
                a.Student.ClassId == classId &&
                a.Date.Date == date.Date)
            .OrderBy(a => a.Student.LastName)
            .ThenBy(a => a.Student.FirstName)
            .ToListAsync();
    }

    public async Task<double> GetStudentAttendancePercentageAsync(Guid studentId, DateTime startDate, DateTime endDate)
    {
        var attendanceRecords = await _entities
            .Where(a => 
                a.StudentId == studentId &&
                a.Date.Date >= startDate.Date &&
                a.Date.Date <= endDate.Date)
            .ToListAsync();

        if (!attendanceRecords.Any())
            return 0;

        var totalDays = attendanceRecords.Count;
        var presentDays = attendanceRecords.Count(a => a.IsPresent);

        return (double)presentDays / totalDays * 100;
    }

    public async Task<Dictionary<Guid, double>> GetClassAttendancePercentageAsync(Guid classId, DateTime startDate, DateTime endDate)
    {
        var students = await _context.Students
            .Where(s => s.ClassId == classId)
            .Select(s => s.Id)
            .ToListAsync();

        var attendanceRecords = await _entities
            .Where(a => 
                students.Contains(a.StudentId) &&
                a.Date.Date >= startDate.Date &&
                a.Date.Date <= endDate.Date)
            .GroupBy(a => a.StudentId)
            .Select(g => new
            {
                StudentId = g.Key,
                TotalDays = g.Count(),
                PresentDays = g.Count(a => a.IsPresent)
            })
            .ToListAsync();

        return attendanceRecords.ToDictionary(
            r => r.StudentId,
            r => r.TotalDays > 0 ? (double)r.PresentDays / r.TotalDays * 100 : 0
        );
    }

    public async Task<bool> HasAttendanceBeenMarkedAsync(Guid studentId, DateTime date)
    {
        return await _entities
            .AnyAsync(a => 
                a.StudentId == studentId && 
                a.Date.Date == date.Date);
    }

    public async Task<IReadOnlyList<Student>> GetAbsentStudentsAsync(DateTime date)
    {
        var absentAttendances = await _entities
            .Include(a => a.Student)
            .ThenInclude(s => s.Class)
            .Where(a => 
                a.Date.Date == date.Date && 
                !a.IsPresent)
            .Select(a => a.Student)
            .OrderBy(s => s.Class.Name)
            .ThenBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();

        return absentAttendances;
    }

    public async Task BulkCreateAttendanceAsync(IEnumerable<Attendance> attendances)
    {
        await _entities.AddRangeAsync(attendances);
    }

    public override async Task<Attendance?> GetByIdAsync(Guid id)
    {
        return await _entities
            .Include(a => a.Student)
            .ThenInclude(s => s.Class)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public override async Task<IReadOnlyList<Attendance>> GetAllAsync()
    {
        return await _entities
            .Include(a => a.Student)
            .ThenInclude(s => s.Class)
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.Student.Class.Name)
            .ThenBy(a => a.Student.LastName)
            .ThenBy(a => a.Student.FirstName)
            .ToListAsync();
    }
}
