using Microsoft.EntityFrameworkCore;
using SmartSchoolManagementSystem.Core.Entities.School;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Infrastructure.Data;

namespace SmartSchoolManagementSystem.Infrastructure.Repositories;

public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Subject?> GetSubjectByCodeAsync(string code)
    {
        return await _entities
            .Include(s => s.Teacher)
            .Include(s => s.Classes)
            .FirstOrDefaultAsync(s => s.Code == code);
    }

    public async Task<IReadOnlyList<Subject>> GetSubjectsByTeacherAsync(Guid teacherId)
    {
        return await _entities
            .Include(s => s.Classes)
            .Where(s => s.TeacherId == teacherId)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Subject>> GetSubjectsByClassAsync(Guid classId)
    {
        var classEntity = await _context.Classes
            .Include(c => c.Subjects)
            .FirstOrDefaultAsync(c => c.Id == classId);

        return classEntity?.Subjects.OrderBy(s => s.Name).ToList() ?? new List<Subject>();
    }

    public async Task<bool> IsSubjectCodeUniqueAsync(string code, Guid? excludeId = null)
    {
        return !await _entities
            .AnyAsync(s => 
                s.Code == code && 
                (!excludeId.HasValue || s.Id != excludeId.Value));
    }

    public async Task<IReadOnlyList<Subject>> SearchSubjectsAsync(string searchTerm)
    {
        return await _entities
            .Include(s => s.Teacher)
            .Include(s => s.Classes)
            .Where(s => 
                s.Name.Contains(searchTerm) ||
                s.Code.Contains(searchTerm) ||
                s.Description.Contains(searchTerm))
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Class>> GetSubjectClassesAsync(Guid subjectId)
    {
        var subject = await _entities
            .Include(s => s.Classes)
            .FirstOrDefaultAsync(s => s.Id == subjectId);

        return subject?.Classes.OrderBy(c => c.Name).ToList() ?? new List<Class>();
    }

    public async Task<int> GetSubjectStudentCountAsync(Guid subjectId)
    {
        var subject = await _entities
            .Include(s => s.Classes)
            .ThenInclude(c => c.Students)
            .FirstOrDefaultAsync(s => s.Id == subjectId);

        return subject?.Classes.Sum(c => c.Students.Count) ?? 0;
    }

    public async Task<bool> IsTeacherQualifiedForSubjectAsync(Guid teacherId, Guid subjectId)
    {
        var teacher = await _context.Teachers
            .Include(t => t.Subjects)
            .FirstOrDefaultAsync(t => t.Id == teacherId);

        if (teacher == null) return false;

        var subject = await _entities.FindAsync(subjectId);
        if (subject == null) return false;

        // Check if teacher's specialization matches the subject
        // This is a simple implementation - you might want to add more complex logic
        return teacher.Specialization.Contains(subject.Name) ||
               teacher.Subjects.Any(s => s.Id == subjectId);
    }

    public override async Task<Subject?> GetByIdAsync(Guid id)
    {
        return await _entities
            .Include(s => s.Teacher)
            .Include(s => s.Classes)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public override async Task<IReadOnlyList<Subject>> GetAllAsync()
    {
        return await _entities
            .Include(s => s.Teacher)
            .Include(s => s.Classes)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
}
