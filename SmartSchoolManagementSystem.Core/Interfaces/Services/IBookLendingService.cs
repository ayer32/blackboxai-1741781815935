using SmartSchoolManagementSystem.Core.DTOs.Library;
using SmartSchoolManagementSystem.Core.Entities.Library;

namespace SmartSchoolManagementSystem.Core.Interfaces.Services;

public interface IBookLendingService : IBaseService<BookLending, BookLendingDto, CreateBookLendingDto, UpdateBookLendingDto>
{
    Task<IReadOnlyList<BookLendingDto>> GetLendingsByBorrowerAsync(Guid borrowerId);
    Task<IReadOnlyList<BookLendingDto>> GetOverdueLendingsAsync();
    Task<IReadOnlyList<BookLendingDto>> GetActiveLendingsAsync();
    Task<decimal> CalculateOverdueFineAsync(Guid lendingId);
    Task<BookLendingDto> ProcessBookReturnAsync(Guid lendingId, bool collectFine = true);
    Task<bool> IsBookAvailableForLendingAsync(Guid bookId);
    Task<IReadOnlyList<BookLendingDto>> GetLendingHistoryByBookAsync(Guid bookId);
    Task<int> GetBorrowerActiveLoansCountAsync(Guid borrowerId);
    Task<LendingSummaryDto> GetLendingSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<BookLendingDto> RenewLendingAsync(Guid lendingId, DateTime newDueDate);
    Task<bool> MarkAsLostAsync(Guid lendingId);
}
