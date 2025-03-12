using Microsoft.EntityFrameworkCore;
using SmartSchoolManagementSystem.Core.Entities.Library;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Infrastructure.Data;

namespace SmartSchoolManagementSystem.Infrastructure.Repositories;

public class BookLendingRepository : BaseRepository<BookLending>, IBookLendingRepository
{
    public BookLendingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<BookLending>> GetLendingsByBorrowerAsync(Guid borrowerId)
    {
        return await _entities
            .Include(bl => bl.Book)
            .Where(bl => bl.BorrowerId == borrowerId)
            .OrderByDescending(bl => bl.BorrowDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<BookLending>> GetOverdueLendingsAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _entities
            .Include(bl => bl.Book)
            .Where(bl => 
                bl.Status == LendingStatus.Borrowed && 
                bl.DueDate.Date < today)
            .OrderBy(bl => bl.DueDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<BookLending>> GetActiveLendingsAsync()
    {
        return await _entities
            .Include(bl => bl.Book)
            .Where(bl => bl.Status == LendingStatus.Borrowed)
            .OrderByDescending(bl => bl.BorrowDate)
            .ToListAsync();
    }

    public async Task<decimal> CalculateOverdueFineAsync(Guid lendingId)
    {
        var lending = await _entities.FindAsync(lendingId);
        if (lending == null || lending.Status != LendingStatus.Borrowed)
            return 0;

        var today = DateTime.UtcNow.Date;
        if (lending.DueDate.Date >= today)
            return 0;

        // Calculate fine: $1 per day overdue
        var daysOverdue = (today - lending.DueDate.Date).Days;
        return daysOverdue * 1.0m;
    }

    public async Task<bool> IsBookAvailableForLendingAsync(Guid bookId)
    {
        var book = await _context.Books.FindAsync(bookId);
        return book?.AvailableCopies > 0;
    }

    public async Task<IReadOnlyList<BookLending>> GetLendingHistoryByBookAsync(Guid bookId)
    {
        return await _entities
            .Where(bl => bl.BookId == bookId)
            .OrderByDescending(bl => bl.BorrowDate)
            .ToListAsync();
    }

    public async Task<int> GetBorrowerActiveLoansCountAsync(Guid borrowerId)
    {
        return await _entities
            .CountAsync(bl => 
                bl.BorrowerId == borrowerId && 
                bl.Status == LendingStatus.Borrowed);
    }

    public override async Task<BookLending?> GetByIdAsync(Guid id)
    {
        return await _entities
            .Include(bl => bl.Book)
            .FirstOrDefaultAsync(bl => bl.Id == id);
    }

    public override async Task<IReadOnlyList<BookLending>> GetAllAsync()
    {
        return await _entities
            .Include(bl => bl.Book)
            .OrderByDescending(bl => bl.BorrowDate)
            .ToListAsync();
    }
}
