namespace SmartSchoolManagementSystem.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IStudentRepository Students { get; }
    ITeacherRepository Teachers { get; }
    IClassRepository Classes { get; }
    ISubjectRepository Subjects { get; }
    IAttendanceRepository Attendance { get; }
    IBookRepository Books { get; }
    IBookLendingRepository BookLendings { get; }
    IDonationRepository Donations { get; }

    Task<int> CompleteAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
