using SmartSchoolManagementSystem.Core.Entities.Library;

namespace SmartSchoolManagementSystem.Core.Interfaces;

public interface IBookLendingRepository : IRepository<BookLending>
{
    Task<IReadOnlyList<BookLending>> GetLendingsByBorrowerAsync(Guid borrowerId);
    Task<IReadOnlyList<BookLending>> GetOverdueLendingsAsync();
    Task<IReadOnlyList<BookLending>> GetActiveLendingsAsync();
    Task<decimal> CalculateOverdueFineAsync(Guid lendingId);
    Task<bool> IsBookAvailableForLendingAsync(Guid bookId);
    Task<IReadOnlyList<BookLending>> GetLendingHistoryByBookAsync(Guid bookId);
    Task<int> GetBorrowerActiveLoansCountAsync(Guid borrowerId);
}
