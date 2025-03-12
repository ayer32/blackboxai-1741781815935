using Microsoft.EntityFrameworkCore;
using SmartSchoolManagementSystem.Core.Entities.School;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Infrastructure.Data;

namespace SmartSchoolManagementSystem.Infrastructure.Repositories;

public class ClassRepository : BaseRepository<Class>, IClassRepository
{
    public ClassRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Class>> GetClassesByAcademicYearAsync(int academicYear)
    {
        return await _entities
            .Include(c => c.ClassTeacher)
            .Include(c => c.Students)
            .Include(c => c.Subjects)
            .Where(c => c.AcademicYear == academicYear)
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Section)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Student>> GetClassStudentsAsync(Guid classId)
    {
        return await _context.Students
            .Where(s => s.ClassId == classId)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Subject>> GetClassSubjectsAsync(Guid classId)
    {
        var classEntity = await _entities
            .Include(c => c.Subjects)
            .FirstOrDefaultAsync(c => c.Id == classId);

        return classEntity?.Subjects.OrderBy(s => s.Name).ToList() ?? new List<Subject>();
    }

    public async Task<int> GetStudentCountAsync(Guid classId)
    {
        return await _context.Students
            .CountAsync(s => s.ClassId == classId);
    }

    public async Task<Teacher?> GetClassTeacherAsync(Guid classId)
    {
        var classEntity = await _entities
            .Include(c => c.ClassTeacher)
            .FirstOrDefaultAsync(c => c.Id == classId);

        return classEntity?.ClassTeacher;
    }

    public async Task<bool> IsClassNameUniqueAsync(string name, string section, int academicYear, Guid? excludeId = null)
    {
        return !await _entities
            .AnyAsync(c => 
                c.Name == name && 
                c.Section == section && 
                c.AcademicYear == academicYear &&
                (!excludeId.HasValue || c.Id != excludeId.Value));
    }

    public async Task<IReadOnlyList<Class>> SearchClassesAsync(string searchTerm)
    {
        return await _entities
            .Include(c => c.ClassTeacher)
            .Include(c => c.Students)
            .Include(c => c.Subjects)
            .Where(c => 
                c.Name.Contains(searchTerm) ||
                c.Section.Contains(searchTerm) ||
                c.ClassTeacher.FirstName.Contains(searchTerm) ||
                c.ClassTeacher.LastName.Contains(searchTerm))
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Section)
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, int>> GetAttendanceStatisticsAsync(Guid classId, DateTime date)
    {
        var students = await _context.Students
            .Where(s => s.ClassId == classId)
            .Select(s => s.Id)
            .ToListAsync();

        var attendanceStats = await _context.Attendances
            .Where(a => 
                students.Contains(a.StudentId) && 
                a.Date.Date == date.Date)
            .GroupBy(a => a.StudentId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Count(a => a.IsPresent));

        return attendanceStats;
    }

    public override async Task<Class?> GetByIdAsync(Guid id)
    {
        return await _entities
            .Include(c => c.ClassTeacher)
            .Include(c => c.Students)
            .Include(c => c.Subjects)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public override async Task<IReadOnlyList<Class>> GetAllAsync()
    {
        return await _entities
            .Include(c => c.ClassTeacher)
            .Include(c => c.Students)
            .Include(c => c.Subjects)
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Section)
            .ToListAsync();
    }
}
