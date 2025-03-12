using Microsoft.EntityFrameworkCore;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Infrastructure.Repositories;

namespace SmartSchoolManagementSystem.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IStudentRepository? _students;
    private ITeacherRepository? _teachers;
    private IClassRepository? _classes;
    private ISubjectRepository? _subjects;
    private IAttendanceRepository? _attendance;
    private IBookRepository? _books;
    private IBookLendingRepository? _bookLendings;
    private IDonationRepository? _donations;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IStudentRepository Students => 
        _students ??= new StudentRepository(_context);

    public ITeacherRepository Teachers =>
        _teachers ??= new TeacherRepository(_context);

    public IClassRepository Classes =>
        _classes ??= new ClassRepository(_context);

    public ISubjectRepository Subjects =>
        _subjects ??= new SubjectRepository(_context);

    public IAttendanceRepository Attendance =>
        _attendance ??= new AttendanceRepository(_context);

    public IBookRepository Books =>
        _books ??= new BookRepository(_context);

    public IBookLendingRepository BookLendings =>
        _bookLendings ??= new BookLendingRepository(_context);

    public IDonationRepository Donations =>
        _donations ??= new DonationRepository(_context);

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.Database.CommitTransactionAsync();
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
