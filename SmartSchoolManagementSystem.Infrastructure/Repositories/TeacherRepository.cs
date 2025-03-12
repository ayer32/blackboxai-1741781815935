using Microsoft.EntityFrameworkCore;
using SmartSchoolManagementSystem.Core.Entities.School;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Infrastructure.Data;

namespace SmartSchoolManagementSystem.Infrastructure.Repositories;

public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
{
    public TeacherRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Teacher?> GetTeacherByTeacherIdAsync(string teacherId)
    {
        return await _entities
            .Include(t => t.Classes)
            .Include(t => t.Subjects)
            .FirstOrDefaultAsync(t => t.TeacherId == teacherId);
    }

    public async Task<IReadOnlyList<Teacher>> SearchTeachersAsync(string searchTerm)
    {
        return await _entities
            .Include(t => t.Classes)
            .Include(t => t.Subjects)
            .Where(t => 
                t.FirstName.Contains(searchTerm) ||
                t.LastName.Contains(searchTerm) ||
                t.TeacherId.Contains(searchTerm) ||
                t.Email.Contains(searchTerm) ||
                t.Specialization.Contains(searchTerm))
            .OrderBy(t => t.LastName)
            .ThenBy(t => t.FirstName)
            .ToListAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null)
    {
        return !await _entities
            .AnyAsync(t => 
                t.Email == email && 
                (!excludeId.HasValue || t.Id != excludeId.Value));
    }

    public async Task<bool> IsTeacherIdUniqueAsync(string teacherId, Guid? excludeId = null)
    {
        return !await _entities
            .AnyAsync(t => 
                t.TeacherId == teacherId && 
                (!excludeId.HasValue || t.Id != excludeId.Value));
    }

    public async Task<IReadOnlyList<Class>> GetTeacherClassesAsync(Guid teacherId)
    {
        return await _context.Classes
            .Include(c => c.Students)
            .Include(c => c.Subjects)
            .Where(c => c.TeacherId == teacherId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Subject>> GetTeacherSubjectsAsync(Guid teacherId)
    {
        return await _context.Subjects
            .Include(s => s.Classes)
            .Where(s => s.TeacherId == teacherId)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<int> GetTeacherClassCountAsync(Guid teacherId)
    {
        return await _context.Classes
            .CountAsync(c => c.TeacherId == teacherId);
    }

    public async Task<int> GetTeacherSubjectCountAsync(Guid teacherId)
    {
        return await _context.Subjects
            .CountAsync(s => s.TeacherId == teacherId);
    }

    public override async Task<Teacher?> GetByIdAsync(Guid id)
    {
        return await _entities
            .Include(t => t.Classes)
            .Include(t => t.Subjects)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public override async Task<IReadOnlyList<Teacher>> GetAllAsync()
    {
        return await _entities
            .Include(t => t.Classes)
            .Include(t => t.Subjects)
            .OrderBy(t => t.LastName)
            .ThenBy(t => t.FirstName)
            .ToListAsync();
    }
}
