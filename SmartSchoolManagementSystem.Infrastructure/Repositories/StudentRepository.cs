using Microsoft.EntityFrameworkCore;
using SmartSchoolManagementSystem.Core.Entities.Library;
using SmartSchoolManagementSystem.Core.Entities.School;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Infrastructure.Data;

namespace SmartSchoolManagementSystem.Infrastructure.Repositories;

public class StudentRepository : BaseRepository<Student>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Student?> GetStudentByStudentIdAsync(string studentId)
    {
        return await _entities
            .Include(s => s.Class)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
    }

    public async Task<IReadOnlyList<Student>> GetStudentsByClassAsync(Guid classId)
    {
        return await _entities
            .Where(s => s.ClassId == classId)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Student>> SearchStudentsAsync(string searchTerm)
    {
        return await _entities
            .Include(s => s.Class)
            .Where(s => 
                s.FirstName.Contains(searchTerm) ||
                s.LastName.Contains(searchTerm) ||
                s.StudentId.Contains(searchTerm) ||
                s.Email.Contains(searchTerm))
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, Guid? excludeId = null)
    {
        return !await _entities
            .AnyAsync(s => 
                s.Email == email && 
                (!excludeId.HasValue || s.Id != excludeId.Value));
    }

    public async Task<bool> IsStudentIdUniqueAsync(string studentId, Guid? excludeId = null)
    {
        return !await _entities
            .AnyAsync(s => 
                s.StudentId == studentId && 
                (!excludeId.HasValue || s.Id != excludeId.Value));
    }

    public async Task<IReadOnlyList<Student>> GetStudentsWithOverdueBooksAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _entities
            .Include(s => s.BookLendings)
            .Where(s => s.BookLendings.Any(bl => 
                bl.Status == LendingStatus.Borrowed && 
                bl.DueDate.Date < today))
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<BookLending>> GetStudentLendingHistoryAsync(Guid studentId)
    {
        return await _context.BookLendings
            .Include(bl => bl.Book)
            .Where(bl => bl.BorrowerId == studentId)
            .OrderByDescending(bl => bl.BorrowDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Attendance>> GetStudentAttendanceHistoryAsync(Guid studentId, DateTime startDate, DateTime endDate)
    {
        return await _context.Attendances
            .Where(a => 
                a.StudentId == studentId &&
                a.Date >= startDate &&
                a.Date <= endDate)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    public override async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _entities
            .Include(s => s.Class)
            .Include(s => s.Attendances)
            .Include(s => s.BookLendings)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public override async Task<IReadOnlyList<Student>> GetAllAsync()
    {
        return await _entities
            .Include(s => s.Class)
            .OrderBy(s => s.LastName)
            .ThenBy(s => s.FirstName)
            .ToListAsync();
    }
}
