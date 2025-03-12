using AutoMapper;
using SmartSchoolManagementSystem.Core.DTOs.Library;
using SmartSchoolManagementSystem.Core.Entities.Library;
using SmartSchoolManagementSystem.Core.Interfaces;
using SmartSchoolManagementSystem.Core.Interfaces.Services;

namespace SmartSchoolManagementSystem.Services;

public class BookLendingService : BaseService<BookLending, BookLendingDto, CreateBookLendingDto, UpdateBookLendingDto>, IBookLendingService
{
    private readonly IBookLendingRepository _bookLendingRepository;
    private readonly IBookRepository _bookRepository;

    public BookLendingService(
        IBookLendingRepository bookLendingRepository,
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper) : base(bookLendingRepository, unitOfWork, mapper)
    {
        _bookLendingRepository = bookLendingRepository;
        _bookRepository = bookRepository;
    }

    public async Task<IReadOnlyList<BookLendingDto>> GetLendingsByBorrowerAsync(Guid borrowerId)
    {
        var lendings = await _bookLendingRepository.GetLendingsByBorrowerAsync(borrowerId);
        return _mapper.Map<IReadOnlyList<BookLendingDto>>(lendings);
    }

    public async Task<IReadOnlyList<BookLendingDto>> GetOverdueLendingsAsync()
    {
        var lendings = await _bookLendingRepository.GetOverdueLendingsAsync();
        return _mapper.Map<IReadOnlyList<BookLendingDto>>(lendings);
    }

    public async Task<IReadOnlyList<BookLendingDto>> GetActiveLendingsAsync()
    {
        var lendings = await _bookLendingRepository.GetActiveLendingsAsync();
        return _mapper.Map<IReadOnlyList<BookLendingDto>>(lendings);
    }

    public async Task<decimal> CalculateOverdueFineAsync(Guid lendingId)
    {
        return await _bookLendingRepository.CalculateOverdueFineAsync(lendingId);
    }

    public async Task<BookLendingDto> ProcessBookReturnAsync(Guid lendingId, bool collectFine = true)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var lending = await _bookLendingRepository.GetByIdAsync(lendingId);
            if (lending == null)
                throw new KeyNotFoundException($"Lending with ID {lendingId} was not found.");

            if (lending.Status != LendingStatus.Borrowed)
                throw new InvalidOperationException("This book has already been returned or marked as lost.");

            // Calculate fine if applicable
            if (collectFine)
            {
                lending.FineAmount = await CalculateOverdueFineAsync(lendingId);
                lending.IsFineCollected = lending.FineAmount > 0;
            }

            lending.ReturnDate = DateTime.UtcNow;
            lending.Status = LendingStatus.Returned;

            // Update book availability
            await _bookRepository.UpdateBookAvailabilityAsync(lending.BookId, 1);

            await _bookLendingRepository.UpdateAsync(lending);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<BookLendingDto>(lending);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> IsBookAvailableForLendingAsync(Guid bookId)
    {
        return await _bookLendingRepository.IsBookAvailableForLendingAsync(bookId);
    }

    public async Task<IReadOnlyList<BookLendingDto>> GetLendingHistoryByBookAsync(Guid bookId)
    {
        var lendings = await _bookLendingRepository.GetLendingHistoryByBookAsync(bookId);
        return _mapper.Map<IReadOnlyList<BookLendingDto>>(lendings);
    }

    public async Task<int> GetBorrowerActiveLoansCountAsync(Guid borrowerId)
    {
        return await _bookLendingRepository.GetBorrowerActiveLoansCountAsync(borrowerId);
    }

    public async Task<LendingSummaryDto> GetLendingSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var activeLendings = await GetActiveLendingsAsync();
        var overdueLendings = await GetOverdueLendingsAsync();
        var allLendings = await _bookLendingRepository.GetAllAsync();

        if (startDate.HasValue && endDate.HasValue)
        {
            allLendings = allLendings
                .Where(l => l.BorrowDate >= startDate && l.BorrowDate <= endDate)
                .ToList();
        }

        var summary = new LendingSummaryDto
        {
            TotalActiveLendings = activeLendings.Count,
            OverdueLendings = overdueLendings.Count,
            TotalFinesCollected = allLendings
                .Where(l => l.IsFineCollected)
                .Sum(l => l.FineAmount ?? 0),
            PendingFines = allLendings
                .Where(l => !l.IsFineCollected && l.FineAmount.HasValue)
                .Sum(l => l.FineAmount ?? 0),
            LendingsByStatus = allLendings
                .GroupBy(l => l.Status)
                .ToDictionary(g => g.Key, g => g.Count()),
            MonthlyLendings = allLendings
                .GroupBy(l => new { l.BorrowDate.Year, l.BorrowDate.Month })
                .Select(g => new MonthlyLendingDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    LendingCount = g.Count(),
                    ReturnCount = g.Count(l => l.Status == LendingStatus.Returned),
                    FinesCollected = g.Where(l => l.IsFineCollected).Sum(l => l.FineAmount ?? 0)
                })
                .OrderByDescending(m => m.Year)
                .ThenByDescending(m => m.Month)
                .ToList()
        };

        return summary;
    }

    public async Task<BookLendingDto> RenewLendingAsync(Guid lendingId, DateTime newDueDate)
    {
        var lending = await _bookLendingRepository.GetByIdAsync(lendingId);
        if (lending == null)
            throw new KeyNotFoundException($"Lending with ID {lendingId} was not found.");

        if (lending.Status != LendingStatus.Borrowed)
            throw new InvalidOperationException("Only active lendings can be renewed.");

        if (newDueDate <= lending.DueDate)
            throw new InvalidOperationException("New due date must be later than the current due date.");

        lending.DueDate = newDueDate;
        await _bookLendingRepository.UpdateAsync(lending);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<BookLendingDto>(lending);
    }

    public async Task<bool> MarkAsLostAsync(Guid lendingId)
    {
        var lending = await _bookLendingRepository.GetByIdAsync(lendingId);
        if (lending == null)
            return false;

        lending.Status = LendingStatus.Lost;
        await _bookLendingRepository.UpdateAsync(lending);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public override async Task<BookLendingDto> CreateAsync(CreateBookLendingDto createDto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (!await IsBookAvailableForLendingAsync(createDto.BookId))
                throw new InvalidOperationException("The book is not available for lending.");

            var activeLoansCount = await GetBorrowerActiveLoansCountAsync(createDto.BorrowerId);
            if (activeLoansCount >= 3) // Maximum 3 active loans per borrower
                throw new InvalidOperationException("Borrower has reached the maximum number of active loans.");

            var lending = _mapper.Map<BookLending>(createDto);
            lending.Status = LendingStatus.Borrowed;

            await _bookLendingRepository.AddAsync(lending);
            await _bookRepository.UpdateBookAvailabilityAsync(createDto.BookId, -1);
            await _unitOfWork.CompleteAsync();
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<BookLendingDto>(lending);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
